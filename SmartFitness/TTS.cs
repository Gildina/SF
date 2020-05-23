using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TTS
{
    class TTS
    {
        SpeechSynthesizer synth = new SpeechSynthesizer();
        MediaElement media = new MediaElement();

        public async void Read(String content)
        {
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(content);
            media.SetSource(stream, stream.ContentType);
            media.Play();
        }

        public TTS()
        {
            ;
        }

        public static async void ttsRead(String content)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            MediaElement media = new MediaElement();
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(content);
            media.SetSource(stream, stream.ContentType);
            media.Play();
        }

        public void StopRead()
        {
            this.media.Stop();
        }
    }
}

