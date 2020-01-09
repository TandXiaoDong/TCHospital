using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPrint
{
    class Patient
    {
        private string pName;
        private string age;
        private string sex;
        private string fetchWindow;
        private List<PrescriptionInfo> prescInfo;

        public string PName
        {
            get { return pName; }
            set { pName = value; }
        }

        public string Age
        {
            get { return age; }
            set { age = value; }
        }

        public string Sex
        {
            get { return sex; }
            set { sex = value; }
        }

        public string FetchWindow
        {
            get { return fetchWindow; }
            set { fetchWindow = value; }
        }

        public List<PrescriptionInfo> PrescInfo
        {
            get { return prescInfo; }
            set { prescInfo = value; }
        }
    }
}
