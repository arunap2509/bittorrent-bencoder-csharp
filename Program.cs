using System.Text;
using Newtonsoft.Json;

var code = "d4:name4:arun3:agei27e4:dictd4:name4:arun3:agei27e4:listl3:abci100ee";

var byteArr = Encoding.UTF8.GetBytes(code);

BEncoder.Decoder decoder = new BEncoder.Decoder("./living.torrent");

var data = decoder.Decode();

var json = JsonConvert.SerializeObject(decoder);

Console.WriteLine(json);

PrintData(data);

void PrintData(object data)
{
    if (data is Dictionary<string, object> dict)
    {
        foreach (var keyValue in dict)
        {
            if (keyValue.Value is string or int)
            {
                Console.WriteLine($"{keyValue.Key} : {keyValue.Value.ToString()}");
            }
            else
            {
                Console.Write($"{keyValue.Key} : ");
                PrintData(keyValue.Value);
            }
        }
    }
    else if (data is List<object> list)
    {
        foreach (var val in list)
        {
            if (val is string or int)
            {
                Console.Write($"{val} ");
            }
            else
            {
                PrintData(val);
            }
        }
    }
}