// See https://aka.ms/new-console-template for more information
using CitServer;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
server.Start();

Console.WriteLine("Server is started");

while (true)
{
    var client = server.AcceptTcpClient();
    Console.WriteLine("tcp is accepted");

    try
    {
        handleClient(client);
    }
    catch (Exception e) {
        Console.WriteLine(e);
    }
}

static void sendResponse(NetworkStream stream, Response response)
{
    var responseText = JsonSerializer.Serialize(response);
    var responsebuffer = Encoding.UTF8.GetBytes(responseText);
    stream.Write(responsebuffer, 0, responsebuffer.Length);
}

static Response createResponse(string status, string body = "")
{
    return new Response
    {
        Status = status,
        Body = body
    };
}

static void handleClient(TcpClient client)
{
    var stream = client.GetStream();

    var buffer = new byte[1024];

    var rcnt = stream.Read(buffer, 0, buffer.Length);

    var request = Encoding.UTF8.GetString(buffer, 0, rcnt);
    Console.WriteLine(request);
    var requesttext = JsonSerializer.Deserialize<Request>(request);
    Console.WriteLine(requesttext);

    string str = "4 ";

    
    
    if (string.IsNullOrEmpty(requesttext?.Date))
    {
        str = str + "Missing Date, ";
    }
    if (!string.IsNullOrEmpty(requesttext?.Date))
    {
        try
        {
            int.Parse(requesttext.Date);
        }
        catch (Exception)
        {
            sendResponse(stream, createResponse("4 Illegal Date"));
        }
    }
    if (string.IsNullOrEmpty(requesttext?.Method))
    {
        str = str + "Missing Method, ";
    }
    else if (string.IsNullOrEmpty(requesttext?.Path))
    {
        str = str + "Missing Resource, ";
    }
    else if (requesttext.Method.Equals("update"))
    {
        if (string.IsNullOrEmpty(requesttext.Body))
        {
            sendResponse(stream, createResponse("4 Missing Body"));
        }
        else
        {
            sendResponse(stream, createResponse("3 Updated"));
        }
    }
    else if (requesttext.Method.Equals("echo"))
    {
        if (string.IsNullOrEmpty(requesttext.Body))
        {
            sendResponse(stream, createResponse("4 Missing Body"));
        }
        else
        {
            sendResponse(stream, createResponse("1 Ok", requesttext.Body));
        }
    }
    else if (requesttext.Method.Equals("read"))
    {
            sendResponse(stream, createResponse("1 Ok"));
    }
    else if (requesttext.Method.Equals("create"))
    {
        if (string.IsNullOrEmpty(requesttext.Body))
        {
            sendResponse(stream, createResponse("4 Missing Body"));
        }
        else
        {
            sendResponse(stream, createResponse("2 Created"));
        }
    }
    else if (requesttext.Method.Equals("delete"))
    {
            sendResponse(stream, createResponse("1 Ok"));
    }
    else if (!(requesttext.Method.Contains("update") || requesttext.Method.Contains("delete") || requesttext.Method.Contains("read") || requesttext.Method.Contains("create") || requesttext.Method.Contains("echo")))
    {
        str = str + "Illegal Method, ";
    }
    if (str.Length > 2)
    {
        
        sendResponse(stream, createResponse(str));
    }

    stream.Close();
}