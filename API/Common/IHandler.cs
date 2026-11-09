namespace API.Common;

/// <summary>
/// Marks the class that runs one slice's use case. Handlers are registered as scoped services by
/// <see cref="FeatureRegistration.AddFeatures"/> and injected into their endpoint.
/// </summary>
public interface IHandler;
