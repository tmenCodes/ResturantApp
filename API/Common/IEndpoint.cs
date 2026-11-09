namespace API.Common;

/// <summary>
/// The HTTP entry point of one feature slice. Implementations are discovered by
/// <see cref="FeatureRegistration.AddFeatures"/> and mapped by <see cref="FeatureRegistration.MapFeatureEndpoints"/>.
/// </summary>
public interface IEndpoint
{
    /// <summary>Maps the slice's route, usually on a group from <see cref="ApiGroups"/>.</summary>
    void Map(IEndpointRouteBuilder app);
}
