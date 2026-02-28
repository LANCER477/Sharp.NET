using SharpKnP321.Networking.Orm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SharpKnP321.Networking.Api
{
    // Data Accessor for API https://www.icalendar37.net/lunar/api/
    internal class MoonApi
    {
        public async Task<MoonPhase> TodayPhaseAsync()
        {
            int day = DateTime.Now.Day;
            var moonApiResponse = await FetchDataAsync(DateTime.Now.Year, DateTime.Now.Month, day);
            return moonApiResponse.Phase[day.ToString()];
        }

        public async Task<MoonPhase> PhaseByDateAsync(DateOnly date)
        {
            return await TodayPhaseAsync();
        }

        private async Task<MoonApiResponse> FetchDataAsync(int year, int month, int day)
        {
            using HttpClient httpClient = new();
            String href = $"https://www.icalendar37.net/lunar/api/?year={year}&month={month}&day={day}&shadeColor=gray&size=150&texturize=true";
            return JsonSerializer.Deserialize<MoonApiResponse>(
                await httpClient.GetStringAsync(href)
            )!;
        }
    }
}
