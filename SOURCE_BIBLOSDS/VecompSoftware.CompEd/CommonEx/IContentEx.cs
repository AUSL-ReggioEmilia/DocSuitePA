﻿using CCypher;
using System;
using VecompSoftware.Common;

namespace VecompSoftware.CompEd
{
    public static class IContentEx
    {

        public static CompEdContentInfo ToCompEdContent(this IContent source)
        {
            return new CompEdContentInfo(source);
        }
        public static ContentInfo ToIdentifiedContent(this IContent source)
        {
            string originalFileName = source.FileName.ToPath().GetFileNameResolveChain();
            return new ContentInfo(originalFileName, source.Content);
        }

    }
}
