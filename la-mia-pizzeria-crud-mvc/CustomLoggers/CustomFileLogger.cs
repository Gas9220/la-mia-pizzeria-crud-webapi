namespace la_mia_pizzeria_crud_mvc.CustomLoggers
{
    public class CustomFileLogger: ICustomLogger
    {
        public void WriteLog(string message, string operation)
        {
            // projectFolder/bin/net7.0/log.txt
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\log.txt", $"{DateTime.Now:dd-MM-yyyy HH:mm:ss} LOG: {message} OPERATION: {operation}\n");
        }
    }
}
