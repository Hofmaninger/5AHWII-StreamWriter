using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StreamWriter.Web.Controllers
{
    [Route("daily-challange")]
    [ApiController]
    public class DailyChallangeController : ControllerBase
    {
        private readonly IDailyChallangeTextGenerator _dailyChallangeTextGenerator;
        public DailyChallangeController(IDailyChallangeTextGenerator dailyChallangeTextGenerator) 
        { 
            _dailyChallangeTextGenerator = dailyChallangeTextGenerator;
        }

        [HttpGet("text")]
        public async Task<string> GetDailyChallangeText()
        {
            string text = await _dailyChallangeTextGenerator.GetText();
            return text;
        }
    }

    public interface IDailyChallangeTextGenerator
    {
        Task<string> GetText();
    }

    public class StaticDailyChallangeTextGenerator : IDailyChallangeTextGenerator
    {
        public Task<string> GetText()
        {
            return Task.FromResult("This is a static text that comes from StaticDailyChallangeGenerator.");
        }
    }

    public class RotatingDailyChallangeTextGenerator : IDailyChallangeTextGenerator
    {
        public readonly string[] _texts;
        public int _textIndex;

        public RotatingDailyChallangeTextGenerator(string[] texts)
        {
            _texts = texts;
            _textIndex = 0;
        }

        public Task<string> GetText()
        {
            string result = _texts[_textIndex];
            _textIndex = (_textIndex + 1) % _textIndex;
            return Task.FromResult(result);
        }
    }
    public class DelayDailyChallangeTextGenerator : IDailyChallangeTextGenerator
    {
        private readonly TimeSpan _delay;
        private readonly IDailyChallangeTextGenerator _inner;

        public DelayDailyChallangeTextGenerator(TimeSpan delay, IDailyChallangeTextGenerator inner)
        {
            _delay = delay;
            _inner = inner;
        }

        public async Task<string> GetText()
        {
            await Task.Delay(_delay);
            return await _inner.GetText();
        }
    }

}