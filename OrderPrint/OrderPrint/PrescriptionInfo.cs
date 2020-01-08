using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPrint
{
    class PrescriptionInfo
    {
        private string prescriptionNO;
        private List<PrescriptionDetl> detlList;

        public string PrescriptionNO
        {
            get { return prescriptionNO; }
            set { prescriptionNO = value; }
        }

        public List<PrescriptionDetl> DetlList
        {
            get { return detlList; }
            set { detlList = value; }
        }
    }
}