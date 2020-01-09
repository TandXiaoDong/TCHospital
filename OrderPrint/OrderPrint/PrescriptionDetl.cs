using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPrint
{
    class PrescriptionDetl
    {
        private string drugName;
        private string drugSpec;
        private string manufactory;
        private string prescriptionQty;
        private string prescriptionUnit;
        private string storageLoc;
        private string useDosage;
        private string useFrequency;
        private string useRoute;
        private string notes;
        private string num_1;
        private string price;

        public string Num_1
        {
            get { return num_1; }
            set { num_1 = value; }
        }
        public string Price
        {
            get { return price; }
            set { price = value; }
        }
        public string DrugName
        {
            get { return drugName; }
            set { drugName = value; }
        }

        public string DrugSpec
        {
            get { return drugSpec; }
            set { drugSpec = value; }
        }

        public string Manufactory
        {
            get { return manufactory; }
            set { manufactory = value; }
        }

        public string PrescriptionQty
        {
            get { return prescriptionQty; }
            set { prescriptionQty = value; }
        }

        public string PrescriptionUnit
        {
            get { return prescriptionUnit; }
            set { prescriptionUnit = value; }
        }

        public string StorageLoc
        {
            get { return storageLoc; }
            set { storageLoc = value; }
        }

        public string UseDosage
        {
            get { return useDosage; }
            set { useDosage = value; }
        }

        public string UseFrequency
        {
            get { return useFrequency; }
            set { useFrequency = value; }
        }

        public string UseRoute
        {
            get { return useRoute; }
            set { useRoute = value; }
        }

        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }
    }
}
