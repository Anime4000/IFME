using System;
using System.IO.Pipes;
using IFME.OSManager;

class EncoderNamedPipe
{
    private string PipeRx, PipeTx;
    public string CmdPipeRx, CmdPipeTx;

    public EncoderNamedPipe()
    {
        PipeRx = $"IFME_RX_{new Guid()}";
        PipeTx = $"IFME_TX_{new Guid()}";

        if (OS.IsWindows)
        {
            CmdPipeRx = $"\\\\.\\pipe\\{PipeRx}";
            CmdPipeTx = $"\\\\.\\pipe\\{PipeTx}";
        }
        else
        {
            CmdPipeRx = PipeRx;
            CmdPipeTx = PipeTx;
        }
    }

    public bool Start()
    {
        var Decoder = new NamedPipeServerStream(PipeRx, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.WriteThrough);
        var Encoder = new NamedPipeServerStream(PipeTx, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.WriteThrough);

        Encoder.WaitForConnection();
        Decoder.WaitForConnection();

        int read;
        byte[] buffer = new byte[0x1000]; // 4KB
        while (Decoder.IsConnected)
        {
            read = Decoder.Read(buffer, 0, buffer.Length);
            Encoder.Write(buffer, 0, read);
        }

        Decoder.Close();
        Encoder.Close();

        return true;
    }
}
