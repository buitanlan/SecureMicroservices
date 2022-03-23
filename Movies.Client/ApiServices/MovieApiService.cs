﻿using System.Text.Json;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movies.Client.Models;

namespace Movies.Client.ApiServices;

public class MovieApiService : IMovieApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MovieApiService(IHttpClientFactory httpClientFactory,IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Movie> CreateMovie()
    {
        throw new NotImplementedException();
    }

    public Task<Movie> DeleteMovie(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Movie> GetMovie(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Movie>?> GetMovies()
    {
        // var apiClientCredentials = new ClientCredentialsTokenRequest
        // {
        //     Address = "https://localhost:5005/connect/token",
        //
        //     ClientId = "movieClient",
        //     ClientSecret = "secret",
        //
        //     // This is the scope our Protected API requires. 
        //     Scope = "movieAPI"
        // };
        // // creates a new HttpClient to talk to our IdentityServer (localhost:5005)
        // var client = new HttpClient();
        //
        // // just checks if we can reach the Discovery document. Not 100% needed but..
        // var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5005");
        // if (disco.IsError)
        // {
        //     return null; // throw 500 error
        // }
        //
        // // 2. Authenticates and get an access token from Identity Server
        // var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);            
        // if (tokenResponse.IsError)
        // {
        //     return null;
        // }
        //
        // // Another HttpClient for talking now with our Protected API
        // var apiClient = new HttpClient();
        //
        // // 3. Set the access_token in the request Authorization: Bearer <token>
        // client.SetBearerToken(tokenResponse.AccessToken);
        //
        // // 4. Send a request to our Protected API
        // var response = await client.GetAsync("https://localhost:5002/api/protected");
        var httpClient = _httpClientFactory.CreateClient("MovieAPIClient");

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/movies");

        var response = await httpClient.SendAsync(
            request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var movieList = JsonSerializer.Deserialize<List<Movie>>(content);
        return movieList;

    }

    public Task<Movie> UpdateMovie(Movie movie)
    {
        throw new NotImplementedException();
    }
    
    public async Task<UserInfoViewModel> GetUserInfo()
    {           
        var idpClient = _httpClientFactory.CreateClient("IDPClient");

        var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();

        if (metaDataResponse.IsError)
        {
            throw new HttpRequestException("Something went wrong while requesting the access token");
        }

        var accessToken = await _httpContextAccessor
            .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

        var userInfoResponse = await idpClient.GetUserInfoAsync(
            new UserInfoRequest
            {
                Address = metaDataResponse.UserInfoEndpoint,
                Token = accessToken
            });

        if (userInfoResponse.IsError)
        {
            throw new HttpRequestException("Something went wrong while getting user info");
        }

        var userInfoDictionary = userInfoResponse.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);

        return new UserInfoViewModel(userInfoDictionary);            
    }
}