using System;
using System.IO;
using UnityEngine.Recorder.FrameRecorder;
using UnityEngine.Recorder.FrameRecorder.DataSource;

namespace UTJ.FrameCapturer.Recorders
{
    [FrameRecorder(typeof(PNGRecorderSettings),"Video", "UTJ/PNG" )]
    public class PNGRecorder : BaseImageRecorder<PNGRecorderSettings>
    {
        fcAPI.fcPngContext m_ctx;

        public override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            if (!Directory.Exists(m_Settings.m_DestinationPath))
                Directory.CreateDirectory(m_Settings.m_DestinationPath);

            m_ctx = fcAPI.fcPngCreateContext(ref m_Settings.m_PngEncoderSettings);
            return m_ctx;
        }

        public override void EndRecording(RecordingSession session)
        {
            m_ctx.Release();
            base.EndRecording(session);
        }

        public override void RecordFrame(RecordingSession session)
        {
            if (m_Sources.Count != 1)
                throw new Exception("Unsupported number of sources");

            var path = BuildOutputPath(session);
            var source = (RenderTextureInput)m_Sources[0];
            var frame = source.outputRT;

            fcAPI.fcLock(frame, (data, fmt) =>
            {
                fcAPI.fcPngExportPixels(m_ctx, path, data, frame.width, frame.height, fmt, 0);
            });
        }

        string BuildOutputPath(RecordingSession session)
        {
            var outputPath = m_Settings.m_DestinationPath;
            if (outputPath.Length > 0 && !outputPath.EndsWith("/"))
                outputPath += "/";
            outputPath +=  (settings as PNGRecorderSettings).m_BaseFileName + recordedFramesCount.ToString("0000") + ".png";
            return outputPath;
        }
    }
}
