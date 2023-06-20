﻿// Morgan Stanley makes this available to you under the Apache License,
// Version 2.0 (the "License"). You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0.
// 
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership. Unless required by applicable law or agreed
// to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions
// and limitations under the License.

using MorganStanley.ComposeUI.Messaging.Protocol.Messages;

namespace MorganStanley.ComposeUI.Messaging.Client.Abstractions;

/// <summary>
/// Represents a connection that can communicate with the server.
/// </summary>
public interface IConnection : IAsyncDisposable
{
    /// <summary>
    /// Opens the connection.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="MessageRouterException">With name <see cref="MessageRouterErrors.ConnectionClosed"/> if the connection was closed by either party while sending the request</exception>
    /// <exception cref="MessageRouterException">With name <see cref="MessageRouterErrors.ConnectionAborted"/> if the connection was closed due to an unexpected error</exception>
    ValueTask SendAsync(Message message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Receives the next message from the server.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="MessageRouterException">With name <see cref="MessageRouterErrors.ConnectionClosed"/> if the connection was closed by either party while sending the request</exception>
    /// <exception cref="MessageRouterException">With name <see cref="MessageRouterErrors.ConnectionAborted"/> if the connection was closed due to an unexpected error</exception>
    ValueTask<Message> ReceiveAsync(CancellationToken cancellationToken = default);
}
