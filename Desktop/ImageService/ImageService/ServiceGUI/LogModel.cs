using System;

public class LogModel : ILogModel
{
    public List<Tuple<String, String>> logs { get; set; }
    public LogModel()
	{
        this.logs = new List<Tuple<String, String>>();
    }
}
