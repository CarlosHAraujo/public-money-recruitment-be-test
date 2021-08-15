using System;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class PutRentalTests
    {
        private readonly HttpClient _client;

        public PutRentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAGetReturnsTheUpdatedRental()
        {
            var request = new RentalBindingModel
            {
                Units = 5,
                PreparationTimeInDays = 2
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var updateRental = new RentalBindingModel
            {
                Units = 8,
                PreparationTimeInDays = 1
            };

            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", updateRental))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(updateRental.Units, getResult.Units);
                Assert.Equal(updateRental.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAGetReturnsTheUpdatedRentalWhenUnitDecreaseIsAffordable()
        {
            var request = new RentalBindingModel
            {
                Units = 5,
                PreparationTimeInDays = 2
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            //Create bookings
            var postBookingRequest1 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 5,
                Start = new DateTime(2021, 08, 15)
            };

            using (var postBookingResponse1 = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest1))
            {
                Assert.True(postBookingResponse1.IsSuccessStatusCode);
            }

            var postBookingRequest2 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 1,
                Start = new DateTime(2021, 08, 18)
            };

            using (var postBookingResponse2 = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest2))
            {
                Assert.True(postBookingResponse2.IsSuccessStatusCode);
            }

            var postBookingRequest3 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 3,
                Start = new DateTime(2021, 08, 16)
            };

            using (var postBookingResponse3 = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest3))
            {
                Assert.True(postBookingResponse3.IsSuccessStatusCode);
            }

            //Update Rental
            var updateRental = new RentalBindingModel
            {
                Units = 3,
                PreparationTimeInDays = 2
            };

            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", updateRental))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(updateRental.Units, getResult.Units);
                Assert.Equal(updateRental.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAPutReturnsErrorWhenBookingOverflowNewUnit()
        {
            var request = new RentalBindingModel
            {
                Units = 2,
                PreparationTimeInDays = 2
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            //Create bookings
            var postBookingRequest1 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 5,
                Start = new DateTime(2021, 08, 15)
            };

            using (var postBookingResponse1 = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest1))
            {
                Assert.True(postBookingResponse1.IsSuccessStatusCode);
            }

            var postBookingRequest2 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 1,
                Start = new DateTime(2021, 08, 18)
            };

            using (var postBookingResponse2 = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest2))
            {
                Assert.True(postBookingResponse2.IsSuccessStatusCode);
            }

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                //Update Rental
                var updateRental = new RentalBindingModel
                {
                    Units = 1,
                    PreparationTimeInDays = 2
                };

                using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", updateRental))
                {
                    Assert.True(putResponse.IsSuccessStatusCode);
                }
            });
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAGetReturnsTheUpdatedRentalWhenPreparationTimeIncreaseIsAffordable()
        {
            var request = new RentalBindingModel
            {
                Units = 1,
                PreparationTimeInDays = 1
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            //Create bookings
            var postBookingRequest1 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 3,
                Start = new DateTime(2021, 08, 15)
            };

            using (var postBookingResponse1 = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest1))
            {
                Assert.True(postBookingResponse1.IsSuccessStatusCode);
            }

            var postBookingRequest2 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 5,
                Start = new DateTime(2021, 08, 20)
            };

            using (var postBookingResponse2 = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest2))
            {
                Assert.True(postBookingResponse2.IsSuccessStatusCode);
            }

            //Update Rental
            var updateRental = new RentalBindingModel
            {
                Units = 1,
                PreparationTimeInDays = 2
            };

            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", updateRental))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(updateRental.Units, getResult.Units);
                Assert.Equal(updateRental.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAPutReturnsErrorWhenBookingOverflowNewPreparationTime()
        {
            var request = new RentalBindingModel
            {
                Units = 1,
                PreparationTimeInDays = 1
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            //Create bookings
            var postBookingRequest1 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 3,
                Start = new DateTime(2021, 08, 15)
            };

            using (var postBookingResponse1 = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest1))
            {
                Assert.True(postBookingResponse1.IsSuccessStatusCode);
            }

            var postBookingRequest2 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 1,
                Start = new DateTime(2021, 08, 19)
            };

            using (var postBookingResponse2 = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest2))
            {
                Assert.True(postBookingResponse2.IsSuccessStatusCode);
            }

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                //Update Rental
                var updateRental = new RentalBindingModel
                {
                    Units = 1,
                    PreparationTimeInDays = 2
                };

                using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", updateRental))
                {
                    Assert.True(putResponse.IsSuccessStatusCode);
                }
            });
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAGetReturnsTheUpdatedRentalWhenBothUpdatesAreAffordable()
        {
            var request = new RentalBindingModel
            {
                Units = 2,
                PreparationTimeInDays = 1
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            //Create bookings
            var postBookingRequest1 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 3,
                Start = new DateTime(2021, 08, 15)
            };

            using (var postBookingResponse1 = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest1))
            {
                Assert.True(postBookingResponse1.IsSuccessStatusCode);
            }

            var postBookingRequest2 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 5,
                Start = new DateTime(2021, 08, 20)
            };

            using (var postBookingResponse2 = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest2))
            {
                Assert.True(postBookingResponse2.IsSuccessStatusCode);
            }

            //Update Rental
            var updateRental = new RentalBindingModel
            {
                Units = 1,
                PreparationTimeInDays = 2
            };

            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", updateRental))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(updateRental.Units, getResult.Units);
                Assert.Equal(updateRental.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }
        }
    }
}
