using IoTSensorManagement.Shared.Interfaces;
using IoTSensorManagement.Workers.Interfaces;

namespace IoTSensorManagement.Workers
{
    public class WorkerManager : IHostedService
	{
		private readonly IWorkerFactory _workerFactory; 
		private readonly ISharedConfigurationProvider _configProvider;
		private readonly List<IHostedService> _workers = new();

		public WorkerManager(IWorkerFactory workerFactory, ISharedConfigurationProvider configProvider)
		{
			_workerFactory = workerFactory;
			_configProvider = configProvider;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			var sensorConfigs = _configProvider.GetSensorConfigurations();
			foreach (var config in sensorConfigs)
			{
				var worker = _workerFactory.CreateWorker(config);
				_workers.Add(worker);
				await worker.StartAsync(cancellationToken);
			}
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			foreach (var worker in _workers)
			{
				await worker.StopAsync(cancellationToken);
			}
		}
	}
}
