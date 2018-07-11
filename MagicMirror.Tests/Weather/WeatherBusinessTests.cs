﻿using MagicMirror.Business.Enums;
using MagicMirror.Business.Models;
using MagicMirror.Business.Services;
using MagicMirror.DataAccess.Entities.Weather;
using MagicMirror.DataAccess.Repos;
using Moq;
using Xunit;

namespace MagicMirror.Tests.Weather
{
    public class WeatherBusinessTests
    {
        private Mock<IWeatherRepo> _mockRepo;
        private Mock<WeatherService> _mockService;

        private string Location = "London";
        private float Kelvin = 290.6f;
        private string Weathertype = "Clear";
        private string Icon = "01d";
        private int Sunrise = 1531281435;
        private int Sunset = 1531340063;

        public WeatherBusinessTests()
        {
            _mockRepo = new Mock<IWeatherRepo>();
            _mockService = new Mock<WeatherService>(_mockRepo.Object);
        }

        [Fact]
        public void Can_Map_From_Entity()
        {

            // Arrange
            var mockEntity = new Mock<WeatherEntity>();
            mockEntity.Setup(x => x.Name).Returns(Location);
            mockEntity.Setup(x => x.Main.Temp).Returns(Kelvin);
            mockEntity.Setup(x => x.Sys.Sunrise).Returns(Sunrise);
            mockEntity.Setup(x => x.Sys.Sunset).Returns(Sunset);

            mockEntity.Setup(x => x.Weather).Returns(() => new[]
            {
                new DataAccess.Entities.Weather.Weather
                {
                    Main = Weathertype
                }
            });

            // Act
            var model = _mockService.Object.MapFromEntity(mockEntity.Object);

            // Assert
            Assert.Equal(Location, model.Location);
            Assert.Equal(Weathertype, model.WeatherType);
            Assert.Equal(Kelvin, model.Temperature);
            Assert.Equal(Sunrise.ToString(), model.Sunrise);
            Assert.Equal(Sunset.ToString(), model.Sunset);
        }

        [Fact]
        public void Can_Calculate_Values()
        {
            // Arrange
            var model = new WeatherModel()
            {
                Sunrise = Sunrise.ToString(),
                Sunset = Sunset.ToString(),
                Temperature = Kelvin,
                Location = Location,
                TemperatureUom = TemperatureUom.Celsius,
                WeatherType = Weathertype,
                Icon = Icon
            };

            // Act
            model.ConvertValues();

            // Assert
            Assert.Equal(17.45, model.Temperature);
            Assert.Equal("03:57", model.Sunrise);
            Assert.Equal("20:14", model.Sunset);
        }
    }
}