using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.DocSuite.Document.DocumentProxy
{
    internal class GrpcFactory : IDisposable
    {
		private readonly ILogger _logger;
		private readonly string _endpoint;
        private readonly string _cert;
        private const short MAX_CHANNEL_NUMBER = 3;
        private readonly ConcurrentDictionary<int, Channel> _activeGrpcChannels;
		private static ICollection<LogCategory> _logCategories;

		protected static IEnumerable<LogCategory> LogCategories
		{
			get
			{
				if (_logCategories == null)
				{
					_logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(DocumentProxyContext));
				}
				return _logCategories;
			}
		}

		public GrpcFactory(
            ILogger logger, 
            string endpoint, 
            string cert)
        {
			_logger = logger;
			_endpoint = endpoint;
            _cert = cert;
            _activeGrpcChannels = new ConcurrentDictionary<int, Channel>();
        }

        internal DocumentProxy.DocumentProxyClient GetDocumentProxyClient()
        {
            int currentChannelIndex = new Random(0).Next(MAX_CHANNEL_NUMBER);

            if (!_activeGrpcChannels.TryGetValue(currentChannelIndex, out Channel currentChannel) || 
                currentChannel.State == ChannelState.Shutdown || currentChannel.State == ChannelState.TransientFailure)
            {
                List<ChannelOption> opts = new List<ChannelOption>()
                {
                       new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1),//without limit
                       new ChannelOption(ChannelOptions.MaxSendMessageLength, -1)//without limit
                    //new ChannelOption(ChannelOptions.MaxReceiveMessageLength, 100 * 1024 * 1024),
                    //new ChannelOption(ChannelOptions.MaxSendMessageLength, 100 * 1024 * 1024)
                };
                if (!string.IsNullOrEmpty(_cert))
                {
                    SslCredentials secureCredentials = new SslCredentials(_cert);
                    currentChannel = new Channel(_endpoint, secureCredentials, opts);
                    _logger.WriteDebug(new LogMessage("GrpcFactory -> activated PEM certificate"), LogCategories);
                }
                else
                {
                    currentChannel = new Channel(_endpoint, ChannelCredentials.Insecure, opts);
                    _logger.WriteDebug(new LogMessage("GrpcFactory -> activated insecure"), LogCategories);
                }
                if (_activeGrpcChannels.ContainsKey(currentChannelIndex))
                {
                    if (_activeGrpcChannels.TryRemove(currentChannelIndex, out _))
                    {
						_logger.WriteError(new LogMessage($"GrpcFactory -> Failed to remove gRPC channel on index {currentChannelIndex} for address {_endpoint}"), LogCategories);
                        throw new Exception($"GrpcFactory -> Failed to remove gRPC channel on index {currentChannelIndex} for address {_endpoint}");
                    }
                }
                if (!_activeGrpcChannels.TryAdd(currentChannelIndex, currentChannel))
                {
                    _logger.WriteError(new LogMessage($"GrpcFactory -> Failed to create gRPC channel on index {currentChannelIndex} for address {_endpoint}"), LogCategories);
                    throw new Exception($"GrpcFactory -> Failed to create gRPC channel on index {currentChannelIndex} for address {_endpoint}");
                }
            }

            return new DocumentProxy.DocumentProxyClient(currentChannel);
        }

        public void Dispose()
        {
            try
            {
                foreach (KeyValuePair<int, Channel> item in _activeGrpcChannels)
                {
                    item.Value.ShutdownAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
				_logger.WriteError(new LogMessage($"GrpcFactory -> Failed to shutdown gRPC channel for address {_endpoint}: {ex.Message}"), ex, LogCategories);
            }
        }
    }
}