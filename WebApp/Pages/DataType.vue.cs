using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Pages
{
    public class DataType : ViewModel
    {
        public DateTime DateValue { get; set; } = DateTime.Now;
        public bool BoolValue { get; set; } = false;
        public decimal DecimalValue { get; set; } = 1.99m;
        public InnerClass Inner = new InnerClass();

        public void Post()
        {
            this.DateValue = DateValue.AddYears(-1);
            this.DecimalValue = -DecimalValue;
            this.BoolValue = !BoolValue;
            this.Inner.NewDate = this.Inner.NewDate.AddYears(1);
        }

        public class InnerClass
        {
            public DateTime NewDate { get; set; } = new DateTime(2000, 1, 1);
        }
    }
}