namespace LabaN2TP_DB
{
    public class Reader
    {
        private int id;
        public int Id { get => id; set => id = value; }

        private string ?name;
        public string ?Name { get => name; set => name = value; }

        private bool isBan;

        public bool IsBan { get { return isBan; } set { isBan = value; } }

        private DateTime date;
        public DateTime Date { get => date; set => date = value; }

        float fine;
        public float Fine { get => fine; set => fine = value; }

       
        public Reader()
        {
            name = "";
            
            isBan = false;

            Date = DateTime.Today;
            fine = 1.0f;
        }


        public Reader(int id, string name, bool isBan, DateTime dateTime, float fine)
        {
            Id = id;
            Name = name;
            IsBan = isBan;
            Date = dateTime;
            Fine = fine;
        }
        public Reader DeepCopy()
        {
            return new Reader(id, Name, IsBan, Date, Fine);
        }

    }

}