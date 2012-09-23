This is my .Net 4.5 branch of the 7Digital API Wrapper. Features are:

 - All API operations are asynchronous and must be awaited.
 - The HTTP handling code uses the System.Net.HttpClient class (new in .Net 4.5) and has several other refactorings for simplicity.
 - Works only in .Net 4.5 and later.

Branches and Releases
=====================

Whilst we make every effort to keep Master as stable as possible. We cannot 
guarantee, it to be in a permanently usable state as this is where active
development is happening. 

We aim to release semver versioned tags regularly. If you want to use a stable 
version please use the latest versioned tag (v1.x.x)

Work is currently happening towards a v2.x.x release on the master branch with
breaking API changes.

Usage
-----

Consuming applications need to provide a concrete implementation of IApiUri and 
IOAuthCredentials in order to authenticate with the 7digital API. Otherwise wrapper 
will throw MissingDependencyException.

Current invocataion:

artist/details endpoint

    Artist artist = Api<Artist>
                        .Get
                        .WithArtistId(1)
                        .Please()

Not all endpoints implemented yet: most of Artist and some of Release.

Notes
-----

A Schema type knows about its endpoint via the [ApiEndpoint] attribute, e.g

    [ApiEndpoint("artist/details")]
    public class Artist{}

Also - you can apply the following to a type to specify if the endpoint requires 
oauth:

    [OAuthSigned]
    public class OAuthRequestToken{}

See example usage console app project for some more examples.
