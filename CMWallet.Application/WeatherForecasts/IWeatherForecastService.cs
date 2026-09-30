namespace CMWallet.Application.WeatherForecasts;

public interface IWeatherForecastService
{
    IReadOnlyList<WeatherForecast> GetForecasts();
}
