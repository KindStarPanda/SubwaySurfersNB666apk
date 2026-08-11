using System;

public abstract class VideoAdProvider
{
	protected Action<VideoAdProvider> _onVideoAvailable;

	protected Action<VideoAdProvider> _onVideoUnavailable;

	protected Action<VideoAdProvider> _onTakeoverBegan;

	protected Action<VideoAdProvider> _onTakeoverEnded;

	protected Action<VideoAdProvider, int> _onVideoWatched;

	public abstract bool isInitialized { get; }

	protected void InvokeHandlerIfNotNull(Action handler)
	{
		if (handler != null)
		{
			handler();
		}
	}

	protected void InvokeHandlerIfNotNull<T>(Action<T> handler, T val)
	{
		if (handler != null)
		{
			handler(val);
		}
	}

	protected void InvokeHandlerIfNotNull<T1, T2>(Action<T1, T2> handler, T1 val1, T2 val2)
	{
		if (handler != null)
		{
			handler(val1, val2);
		}
	}

	public abstract void Init();

	public abstract bool PlayVideoIfAvailable(int defaultReward);

	public void AddVideoAvailableHandler(Action<VideoAdProvider> handler)
	{
		_onVideoAvailable = (Action<VideoAdProvider>)Delegate.Combine(_onVideoAvailable, handler);
	}

	public void RemoveVideoAvailableHandler(Action<VideoAdProvider> handler)
	{
		_onVideoAvailable = (Action<VideoAdProvider>)Delegate.Remove(_onVideoAvailable, handler);
	}

	public void AddVideoUnavailableHandler(Action<VideoAdProvider> handler)
	{
		_onVideoUnavailable = (Action<VideoAdProvider>)Delegate.Combine(_onVideoUnavailable, handler);
	}

	public void RemoveVideoUnavailableHandler(Action<VideoAdProvider> handler)
	{
		_onVideoUnavailable = (Action<VideoAdProvider>)Delegate.Remove(_onVideoUnavailable, handler);
	}

	public void AddTakeoverBeganHandler(Action<VideoAdProvider> handler)
	{
		_onTakeoverBegan = (Action<VideoAdProvider>)Delegate.Combine(_onTakeoverBegan, handler);
	}

	public void RemoveTakeoverBeganHandler(Action<VideoAdProvider> handler)
	{
		_onTakeoverBegan = (Action<VideoAdProvider>)Delegate.Remove(_onTakeoverBegan, handler);
	}

	public void AddTakeoverEndedHandler(Action<VideoAdProvider> handler)
	{
		_onTakeoverEnded = (Action<VideoAdProvider>)Delegate.Combine(_onTakeoverEnded, handler);
	}

	public void RemoveTakeoverEndedHandler(Action<VideoAdProvider> handler)
	{
		_onTakeoverEnded = (Action<VideoAdProvider>)Delegate.Remove(_onTakeoverEnded, handler);
	}

	public void AddVideoWatchedHandler(Action<VideoAdProvider, int> handler)
	{
		_onVideoWatched = (Action<VideoAdProvider, int>)Delegate.Combine(_onVideoWatched, handler);
	}

	public void RemoveVideoWatchedHandler(Action<VideoAdProvider, int> handler)
	{
		_onVideoWatched = (Action<VideoAdProvider, int>)Delegate.Remove(_onVideoWatched, handler);
	}
}
