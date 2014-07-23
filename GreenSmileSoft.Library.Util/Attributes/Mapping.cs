using System;

namespace GreenSmileSoft.Library.Util.Attributes
{
    public class Mapping :Attribute
    {
        public string Name { get; set; }
        public Mapping(string name)
        {
            Name = name;
        }
    }
}
