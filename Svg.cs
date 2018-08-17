using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace PrickleParser{
            
    public class Svg{
        
        private static XmlDocument doc;

        /// <summary>
        /// Removes empty d attributes in path elements.
        /// </summary>
        /// <param name="input">
        ///     Will be ignored if doc is set as it will assume you want to work on current doc.
        ///     If you want to change to a new svg call the ResetSvg method.
        /// </param>
        public static List<XmlNode> GetEmptyPaths(){

            XmlNodeList pathElements = doc.GetElementsByTagName("path");
            List<XmlNode> emptyPaths = new List<XmlNode>();
            foreach(XmlNode element in pathElements){
                XmlAttributeCollection attributes = element.Attributes;
                XmlNode node = attributes.GetNamedItem("d");
                if (node != null && node.Value.Length == 0){
                    emptyPaths.Add(element);
                }
            }
            return emptyPaths;
        }

        public static void RemoveEmptyPaths(string input){
            if (doc == null){
                doc = new XmlDocument();
                doc.Load(input);
            }
            RemoveParentsOf(GetEmptyPaths());
        }
        
        public static void RenderEmptyPathsExplicit(string input){
            if (doc == null){
                doc = new XmlDocument();
                doc.Load(input);
            }

            foreach(XmlNode emptyPath in GetEmptyPaths()){
                XmlAttributeCollection attributes = emptyPath.Attributes;
                XmlNode dAtt = attributes.GetNamedItem("d");
                dAtt.InnerText = "M 0 0 Z";
            }
        }

        public static void RemoveParentsOf(List<XmlNode> nodes){
            foreach(XmlNode node in nodes){
                if (node.ParentNode != null){
                    node.ParentNode.RemoveAll();
                }
            }
        }

        /// <summary>
        /// Resets current svg so changes will be lost and any input paths to methods will create a new document.
        /// </summary>
        public static void ResetSvg(){
            doc = null;
        }

        /// <summary>
        /// Changes all RGB values to hex values.
        /// </summary>
        /// <param name="input">
        ///     Will be ignored if doc is set as it will assume you want to work on current doc.
        ///     If you want to change to a new svg call the ResetSvg method.
        /// </param>
        public static void RenderRgbAsHex(string input){
            if (doc == null){
                doc = new XmlDocument();
                doc.Load(input);
            }
            
            HashSet<string> potentialAttributes = new HashSet<string>(){"fill", "stroke", "style", "g", "path"};
            
            List<XmlNode> allNodes = GetAllNodes(doc.DocumentElement, new List<XmlNode>());

            string pattern = @"(?<pre>.*?)(?<rgb>rgb\([0-9,.%]+\))(?<post>.*)";
            Regex reg = new Regex(pattern);
            foreach(XmlNode node in allNodes){
                foreach(XmlAttribute att in node.Attributes){
                    if (!potentialAttributes.Contains(att.Name.ToLower())){
                        continue;
                    }

                    Match match = reg.Match(att.InnerText); // todo match multiple instances for style?
                    if (match.Success){
                        StringBuilder sb = new StringBuilder();
                        sb.Append(match.Groups["pre"]);
                        sb.Append(RgbToHex(match.Groups["rgb"].Value));
                        sb.Append(match.Groups["post"]);
                        att.InnerText = sb.ToString();
                    }
                }
            }
        }

        private static string RgbToHex(string rgb){
            // extracts rgb values as well as rgb group including all 3 values to check for percentage format.
            string pattern = @".*?(?<rgb>(?<r>[0-9.]+).*?(?<g>[0-9.]+).*?(?<b>[0-9.]+)).*";
            Regex reg = new Regex(pattern);
            Match match = reg.Match(rgb);

            string hex;
            // in percentage format - 0-100
            if (match.Success && match.Groups["rgb"].Value.Contains("%")){
                // converts rgb percentage format > decimal > rgb integer value
                hex = string.Format("#{0:X2}{1:X2}{2:X2}",
                    (int) Math.Round(float.Parse(match.Groups["r"].Value, CultureInfo.InvariantCulture.NumberFormat) / 100 * 255, 0),
                    (int) Math.Round(float.Parse(match.Groups["g"].Value, CultureInfo.InvariantCulture.NumberFormat) / 100 * 255, 0),
                    (int) Math.Round(float.Parse(match.Groups["b"].Value, CultureInfo.InvariantCulture.NumberFormat) / 100 * 255, 0));
            }
            // int rgb 0-255 format
            else if (match.Success){
                hex = string.Format("{0:X2}{1:X2}{2:X2}",
                    int.Parse(match.Groups["r"].Value, CultureInfo.InvariantCulture.NumberFormat),
                    int.Parse(match.Groups["g"].Value, CultureInfo.InvariantCulture.NumberFormat),
                    int.Parse(match.Groups["b"].Value, CultureInfo.InvariantCulture.NumberFormat));
            }
            else{
                throw new UnexpectedRgbValuesException("Unable to convert rgb to hex has rgb line is not in an expected format");
            }

            return hex;
        }

        private static List<XmlNode> GetAllNodes(XmlNode currentNode, List<XmlNode> allNodes){
            foreach(XmlNode child in currentNode.ChildNodes){
                allNodes.Add(child);
                GetAllNodes(child, allNodes);
            }
            return allNodes;
        }
        
        /// <summary>
        /// Writes svg file to output path. Can only be called if the svg has been initialised through the other
        /// static methods and therefore not null.
        /// </summary>
        /// <param name="output"></param>
        /// <exception cref="NoSvgInitialisedException"></exception>
        public static void WriteSvg(string output) {
            if (doc == null){
                throw new  NoSvgInitialisedException("You must call one of the Svg transformation methods to initialise" +
                                                     "before writing the svg output.");
            }
            doc.Save(output);
        }

        public static XmlDocument Doc => doc;
    }
    
   

    class NoSvgInitialisedException : Exception{
        public NoSvgInitialisedException(string message) : base(message){}
    }

    class UnexpectedRgbValuesException : Exception{
        public UnexpectedRgbValuesException(string message) : base(message){}
    }
}