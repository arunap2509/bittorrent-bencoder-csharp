using System.Text;

namespace BEncoder;

public class Decoder
{
    private readonly string _filename;

    public Decoder(string filename)
    {
        _filename = filename;
    }

    public object Decode()
    {
        byte[] fileDate = File.ReadAllBytes(_filename);

        if (Encoding.UTF8.GetChars(new byte[] { fileDate[0] })[0] != 'd')
        {
            throw new Exception("file is not in valid format");
        }

        var (data, _) = HandleDict(fileDate, 0);

        return data;
    }

    public Dictionary<string, object> Decode(byte[] data)
    {
        var (res, _) = HandleDict(data, 0);

        return res as Dictionary<string, object>;
    }

    public (object, int) DecodeKey(byte[] data, int from)
    {
        var numberList = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        return Encoding.UTF8.GetChars(new byte[] { data[from] })[0] switch
        {
            'i' => HandleInteger(data, from),
            'd' => HandleDict(data, from),
            'l' => HandleList(data, from),
            var x when numberList.Contains(x) => HandleString(data, from),
            _ => throw new Exception("invalid type")
        };
    }

    public (object, int) HandleDict(byte[] data, int from)
    {
        var response = new Dictionary<string, object>();
        var idx = 0;

        for (idx = from + 1; idx < data.Count();)
        {
            if (Encoding.UTF8.GetChars(new byte[] { data[idx] })[0] == 'e')
            {
                return (response, idx + 1);
            }

            var (key, len) = ProcessKey(data, idx);

            idx = len;

            var (value, vlen) = ProcessValue(data, idx);

            idx = vlen;

            response[(string)key] = value;
        }

        return (response, idx);
    }

    private (object, int) ProcessKey(byte[] data, int from)
    {
        return HandleString(data, from);
    }

    private (object, int) ProcessValue(byte[] data, int from)
    {
        return DecodeKey(data, from);
    }

    private (object, int) HandleString(byte[] data, int from)
    {
        var idx = from;
        var numStr = "";

        while (Encoding.UTF8.GetChars(new byte[] { data[idx] })[0] != ':')
        {
            numStr += Encoding.UTF8.GetChars(new byte[] { data[idx] })[0];
            idx++;
        }

        idx++;

        var number = Convert.ToInt32(numStr);

        if (number > 1000)
        {
            return (BitConverter.ToString(data[idx..(idx + number)]).Replace("-", string.Empty), idx + number);
        }

        return (Encoding.UTF8.GetString(data[idx..(idx + number)]), idx + number);
    }

    private (object, int) HandleInteger(byte[] data, int from)
    {
        var idx = from + 1;
        var numStr = "";

        while (Encoding.UTF8.GetChars(new byte[] { data[idx] })[0] != 'e')
        {
            numStr += Encoding.UTF8.GetChars(new byte[] { data[idx] })[0];
            idx++;
        }

        idx++;

        var number = Convert.ToInt32(numStr);

        return (number, idx);
    }

    private (object, int) HandleList(byte[] data, int from)
    {
        var response = new List<object>();
        var idx = from + 1;

        while (Encoding.UTF8.GetChars(new byte[] { data[idx] })[0] != 'e')
        {
            var (val, len) = ProcessValue(data, idx);

            idx = len;

            response.Add(val);
        }

        return (response, idx + 1);
    }
}
