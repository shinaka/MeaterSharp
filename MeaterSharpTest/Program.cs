// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Meater Test");
Console.Write("E-Mail: ");
string? email = Console.ReadLine();

if(email == null)
{
    return;
}

StringBuilder password = new StringBuilder();

Console.Write("\nPassword: ");

while(true)
{
    ConsoleKeyInfo key = Console.ReadKey(true);
    if(key.Key == ConsoleKey.Backspace && password.Length > 0)
    {
        password.Remove(password.Length - 1, 1);
        continue;
    }

    if(key.KeyChar == '\r')
    {
        break;
    }
    
    password.Append(key.KeyChar);
}

Console.WriteLine("\n\nAttempting to log in to Meater API!");

bool bLoginSuccess = await MeaterSharp.Core.Init(email, password.ToString());

Console.WriteLine("Login Result: " + bLoginSuccess.ToString() + "\n");

if (bLoginSuccess)
{
    Console.WriteLine("Getting Devices: ");

    MeaterSharp.Types.MeaterDevices Devices = await MeaterSharp.Core.GetDevices();

    if (Devices != null)
    {
        string DevicesJson = Newtonsoft.Json.JsonConvert.SerializeObject(Devices);
        Console.WriteLine(DevicesJson);

        if (Devices.GetDevices().Length > 0)
        {
            MeaterSharp.Types.MeaterDevice Device = await MeaterSharp.Core.GetDevice(Devices.GetDevices()[0].GetId());
            string DeviceJson = Newtonsoft.Json.JsonConvert.SerializeObject(Device);
            Console.WriteLine(DeviceJson);
        }
        else
        {
            Console.WriteLine("No active devices in MeaterCloud");
        }
    }
    else
    {
        Console.WriteLine("Empty devices response");
    }
}