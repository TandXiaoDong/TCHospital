using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPrint
{
    class BasketStock
    {
        private string checkCode;
        private string lightCode;
        private string closeCode;
        private bool basket;
        private bool light;

        public string CheckCode
        {
            set { checkCode = value; }
            get { return checkCode; }
        }

        public string LightCode
        {
            set { lightCode = value; }
            get { return lightCode; }
        }

        public string CloseCode
        {
            set { closeCode = value; }
            get { return closeCode; }
        }

        public bool Basket
        {
            set { basket = value; }
            get { return basket; }
        }

        public bool Light
        {
            set { light = value; }
            get { return light; }
        }

    }
}
