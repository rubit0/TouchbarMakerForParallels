using System;

namespace TouchbarMaker.ViewModels
{
    public class ElementViewModel
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public ElementViewModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}