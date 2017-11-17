namespace EmitExample
{
    public class Data
    {
        public int IntValue { get; set; }

        public string StringValue { get; set; }
    }

    public class Data2
    {
        public int IntValue { get; }

        public string StringValue { get; }

        public Data2(int intValue, string stringValue)
        {
            IntValue = intValue;
            StringValue = stringValue;
        }
    }

    public enum MyEnum
    {
        Zero, One, Two
    }

    public class EnumPropertyData
    {
        public MyEnum EnumValue { get; set; }
    }

    public struct MyStruct
    {
        public int X { get; set; }

        public int Y { get; set; }
    }

    public class StructPropertyData
    {
        public MyStruct StructValue { get; set; }
    }
}
