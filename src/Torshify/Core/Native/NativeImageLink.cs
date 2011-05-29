using System;

namespace Torshify.Core.Native
{
    internal class NativeImageLink : NativeLink, ILink<IImage>
    {
        #region Fields

        private Lazy<IImage> _image;

        #endregion Fields

        #region Constructors

        public NativeImageLink(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public override object Object
        {
            get { return _image.Value; }
        }

        IImage ILink<IImage>.Object
        {
            get { return (IImage)Object; }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _image = new Lazy<IImage>(() =>
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return new NativeImageFromLink(Session,
                                                   Spotify.sp_image_create_from_link(Session.GetHandle(), Handle));
                }
            });
        }

        #endregion Public Methods

        #region Nested Types

        private class NativeImageFromLink : NativeImage
        {
            #region Fields

            private readonly IntPtr _linkHandle;

            #endregion Fields

            #region Constructors

            public NativeImageFromLink(ISession session, IntPtr linkHandle)
                : base(session, string.Empty)
            {
                _linkHandle = linkHandle;
            }

            #endregion Constructors

            #region Public Methods

            public override void Initialize()
            {
                try
                {
                    lock (Spotify.Mutex)
                    {
                        Handle = Spotify.sp_image_create_from_link(Session.GetHandle(), _linkHandle);
                    }

                    _data = new Lazy<byte[]>(GetImageData);
                    _imageLoaded = OnImageLoadedCallback;

                    lock (Spotify.Mutex)
                    {
                        Spotify.sp_image_add_ref(Handle);
                        Spotify.sp_image_add_load_callback(Handle, _imageLoaded, IntPtr.Zero);
                    }
                }
                catch
                {

                }
            }

            #endregion Public Methods
        }

        #endregion Nested Types
    }
}