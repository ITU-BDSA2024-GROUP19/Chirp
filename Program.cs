using System.Collections;

StreamReader reader = File.OpenText("/home/newnorthstar/Chirp.CLI/chirp_cli_db.csv");
reader.ReadLine();
string line;

while ((line = reader.ReadLine()) != null)
{
    ArrayList list = new ArrayList(line.Split(','));
    string username = (string) list[0];
    list.RemoveAt(0);
    string timecode = (string)list[list.Count - 1];
    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(int.Parse(timecode));
    DateTime dateTime = dateTimeOffset.UtcDateTime;
    timecode = dateTime.ToString();
    list.RemoveAt(list.Count-1);
    string message = "";
    if (list.Count > 1)
    {
        while (list.Count != 0)
        {
            message = message + (string)list[0] + ",";
            list.RemoveAt(0);
        }

        message = message.Substring(1, message.Length - 3);
    }
    else
    {
        message = (string)list[0];
        message = message.Substring(1, message.Length - 2);
    }
    Console.WriteLine(username + " @ " + timecode + ": " + message);
}