using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using CB.Threading.Tasks;


namespace CB.Net.Socket
{
    public static class SocketHelper
    {
        #region Methods
        public static Task<System.Net.Sockets.Socket> AcceptAsync(this System.Net.Sockets.Socket socket)
        {
            var tcs = new TaskCompletionSource<System.Net.Sockets.Socket>();
            socket.BeginAccept(iar =>
            {
                tcs.SetResult(socket.EndAccept(iar));
            }, null);
            return tcs.Task;
        }
        /*public static Task<System.Net.Sockets.Socket> AcceptAsync(this System.Net.Sockets.Socket socket)
            => Task<System.Net.Sockets.Socket>.Factory.FromAsync(socket.BeginAccept, socket.EndAccept, null);*/

        public static Task<System.Net.Sockets.Socket> AcceptAsync(this System.Net.Sockets.Socket socket, int receiveSize)
            => Task.Factory.FromAsync(socket.BeginAccept, socket.EndAccept, receiveSize, null);

        public static Task<System.Net.Sockets.Socket> AcceptAsync(this System.Net.Sockets.Socket socket,
            System.Net.Sockets.Socket acceptSocket, int receiveSize)
            => Task.Factory.FromAsync(socket.BeginAccept, socket.EndAccept, acceptSocket, receiveSize, null);

        public static Task ConnectAsync(this System.Net.Sockets.Socket socket, EndPoint remotEndPoint)
            => Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, remotEndPoint, null);

        public static Task ConnectAsync(this System.Net.Sockets.Socket socket, IPAddress address, int port)
            => Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, address, port, null);

        public static Task ConnectAsync(this System.Net.Sockets.Socket socket, IPAddress[] addresses, int port)
            => Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, addresses, port, null);

        public static Task ConnectAsync(this System.Net.Sockets.Socket socket, string host, int port)
            => Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, host, port, null);

        public static Task DisconnectAsync(this System.Net.Sockets.Socket socket, bool resuseSocket)
            => Task.Factory.FromAsync(socket.BeginDisconnect, socket.EndDisconnect, resuseSocket, null);

        public static Task<int> ReceiveAsync(this System.Net.Sockets.Socket socket, byte[] buffer, int offset, int size,
            SocketFlags socketFlags)
            => Task.Factory.FromAsync(socket.BeginReceive, socket.EndReceive, buffer, offset, size, socketFlags,
                null);

        public static Task<int> ReceiveAsync(this System.Net.Sockets.Socket socket, IList<ArraySegment<byte>> buffers,
            SocketFlags socketFlags)
            => Task.Factory.FromAsync(socket.BeginReceive, socket.EndReceive, buffers, socketFlags, null);

        public static Task<int> SendAsync(this System.Net.Sockets.Socket socket, byte[] buffer, int offset, int size,
            SocketFlags socketFlags)
            => Task.Factory.FromAsync(socket.BeginSend, socket.EndSend, buffer, offset, size, socketFlags, null);

        public static Task<int> SendAsync(this System.Net.Sockets.Socket socket, IList<ArraySegment<byte>> buffers,
            SocketFlags socketFlags)
            => Task.Factory.FromAsync(socket.BeginSend, socket.EndSend, buffers, socketFlags, null);

        public static Task SendFileAsync(this System.Net.Sockets.Socket socket, string fileName)
            => Task.Factory.FromAsync(socket.BeginSendFile, socket.EndSendFile, fileName, null);

        public static Task SendFileAsync(this System.Net.Sockets.Socket socket, string fileName, byte[] preBuffer,
            byte[] postBuffer, TransmitFileOptions flags)
            => Task.Factory.FromAsync(socket.BeginSendFile, socket.EndSendFile, fileName, preBuffer, postBuffer, flags,
                null);

        public static Task<int> SendToAsync(this System.Net.Sockets.Socket socket, byte[] buffer, int offset, int size,
            SocketFlags socketFlags, EndPoint remoteEndPoint)
            => Task.Factory.FromAsync(socket.BeginSendTo, socket.EndSendTo, buffer, offset, size, socketFlags,
                remoteEndPoint, null);
        #endregion
    }
}


// REF: https://msdn.microsoft.com/en-us/library/system.net.sockets.socket(v=vs.110).aspx
// REF: https://msdn.microsoft.com/en-us/library/hh873178.aspx
// REF: http://stackoverflow.com/questions/19170647/task-factory-fromasync-with-out-parameters-in-end-method

// TODO: ref/out on Begin