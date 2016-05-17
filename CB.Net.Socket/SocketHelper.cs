using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace CB.Net.Socket
{
    public static class TaskFactoryHelper
    {
        #region Methods
        public static Task FromAsync<T1, T2, T3, T4>(this TaskFactory taskFactory,
            Func<T1, T2, T3, T4, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, object state)
        {
            var tcs = new TaskCompletionSource<object>();
            beginMethod.Invoke(arg1, arg2, arg3, arg4, iar =>
            {
                endMethod.Invoke(iar);
                tcs.SetResult(null);
            }, state);
            return tcs.Task;
        }

        public static Task FromAsync<T1, T2, T3, T4, T5>(this TaskFactory taskFactory,
            Func<T1, T2, T3, T4, T5, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, object state)
        {
            var tcs = new TaskCompletionSource<object>();
            beginMethod.Invoke(arg1, arg2, arg3, arg4, arg5, iar =>
            {
                endMethod.Invoke(iar);
                tcs.SetResult(null);
            }, state);
            return tcs.Task;
        }

        public static Task<TResult> FromAsync<T1, T2, T3, T4, TResult>(this TaskFactory taskFactory,
            Func<T1, T2, T3, T4, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, object state)
        {
            var tcs = new TaskCompletionSource<TResult>();
            beginMethod.Invoke(arg1, arg2, arg3, arg4, iar => { tcs.SetResult(endMethod.Invoke(iar)); }, state);
            return tcs.Task;
        }

        public static Task<TResult> FromAsync<T1, T2, T3, T4, T5, TResult>(this TaskFactory taskFactory,
            Func<T1, T2, T3, T4, T5, AsyncCallback, object, IAsyncResult> beginMethod,
            Func<IAsyncResult, TResult> endMethod,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, object state)
        {
            var tcs = new TaskCompletionSource<TResult>();
            beginMethod.Invoke(arg1, arg2, arg3, arg4, arg5, iar => { tcs.SetResult(endMethod.Invoke(iar)); }, state);
            return tcs.Task;
        }
        #endregion
    }

    public static class SocketHelper
    {
        #region Methods
        public static Task<System.Net.Sockets.Socket> AcceptAsync(this System.Net.Sockets.Socket socket)
            => Task.Factory.FromAsync(socket.BeginAccept, socket.EndAccept, null);

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