using System.Linq;
using System.Text;

namespace PrickleParser{
    public class Styler{
        
        public static void StyleWord(ChunkMetrics chunkMetric, bool openOnly = false){
            string tmp = chunkMetric.WordStr;
            chunkMetric.WordStr = "";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<font=\"{0}\"><size={1}>", chunkMetric.FontFamily, chunkMetric.GetFontSize());

            if(chunkMetric.Bold){
                sb.Append("<b>");
            }

            if (chunkMetric.Italic){
                sb.Append("<i>");
            }

            if (chunkMetric.Strikethrough){
                sb.Append("<s>");
            }

            if (chunkMetric.Underline){
                sb.Append("<u>");
            }

            if (chunkMetric.Superscript){
                sb.Append("<sup>");
            }
            else if (chunkMetric.Subscript){
                sb.Append("<sub>");
            }

            if (openOnly){
                return;
            }

            if (chunkMetric.Subscript){
                sb.Append("</sub>");
            }
            else if (chunkMetric.Superscript){
                sb.Append("</sup>");
            }

            if (chunkMetric.Underline){
                sb.Append("</u>");
            }
            
            sb.Append(chunkMetric);
            
            if (chunkMetric.Strikethrough){
                sb.Append("</s>");
            }
            
            if(chunkMetric.Bold){
                sb.Append("</b>");
            }

            if (chunkMetric.Italic){
                sb.Append("</i>");
            }
            
            sb.Append("</font>");
            chunkMetric.WordStr = tmp;
        }

        /// <summary>
        /// Style line instead of word by word. Some tags such as bold, italic, font etc will be added to multiple words
        /// Less common styles added on each word it applies.
        /// </summary>
        /// <param name="line"></param>
        public static void StyleLine(LineMetrics line){
            StringBuilder sb = new StringBuilder();
            ChunkMetrics prev = line.WordMetrices.ElementAt(0);
            StyleWord(prev, true);

            for(int i = 1; i < line.WordMetrices.Count; i++){
                StringBuilder closeTags = new StringBuilder();
                
                if (line.WordMetrices.ElementAt(i).FontFamily != prev.FontFamily){
                    
                }
                
                if (line.WordMetrices.ElementAt(i).GetFontSize() != prev.GetFontSize()){
                    
                }

                if (line.WordMetrices.ElementAt(i).Bold){
                    if (!prev.Bold){
                        sb.Append("<b>");
                    }
                }
                else{
                    if (prev.Bold){
                        closeTags.Append("</b>");
                    }
                }
                
                if (line.WordMetrices.ElementAt(i).Italic){
                    
                }

                // The bottom four decorating will be less common and so added word by word
                if (line.WordMetrices.ElementAt(i).Strikethrough){
                    sb.Append("<s>");
                    closeTags.Append("</s>");
                }

                if (line.WordMetrices.ElementAt(i).Underline){
                    sb.Append("<u>");
                    closeTags.Append("</u>");
                }

                if (line.WordMetrices.ElementAt(i).Superscript){
                    sb.Append("<sup>");
                    closeTags.Append("</sup");
                }

                if (line.WordMetrices.ElementAt(i).Subscript){
                    sb.Append("<sub>");
                    closeTags.Append("</sub>");
                }
                
                sb.Append(line.WordMetrices.ElementAt(i));
                sb.Append(closeTags);
            }
        }
    }
}