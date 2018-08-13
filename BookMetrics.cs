namespace PrickleParser{
    
    public class BookMetrics{
        private int nbPages;
        private string author;
        private string title;
        private string date;
        private string publisher;

        public BookMetrics(){
            
        }

        public int NbPages{
            get => nbPages;
            set => nbPages = value;
        }

        public string Author{
            get => author;
            set => author = value;
        }

        public string Title{
            get => title;
            set => title = value;
        }

        public string Date{
            get => date;
            set => date = value;
        }

        public string Publisher{
            get => publisher;
            set => publisher = value;
        }
    }
}