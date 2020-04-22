using Limilabs.Client.IMAP;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
using Limilabs.Mail.MIME;

using System;
using System.Collections.Generic;
using System.Linq;

namespace VecompSoftware.MailManager
{
  public class MailWrapper
  {
    #region Fields

    private readonly IMail _iMail;
    private readonly MessageInfo _messageInfo;

    #endregion

    #region Constructors

    public MailWrapper(IMail mail)
    {
      _iMail = mail;
    }

    public MailWrapper(MessageInfo mail)
    {
      _messageInfo = mail;
    }

    #endregion

    #region Properties

    public string Subject
    {
      get
      {
        if (_messageInfo != null) return _messageInfo.Envelope.Subject;
        return _iMail != null ? _iMail.Subject : string.Empty;
      }
    }

    public IList<MailBox> From
    {
      get
      {
        if (_messageInfo != null)
          return _messageInfo.Envelope.From;

        return _iMail != null ? _iMail.From : new List<MailBox>();
      }
    }

    public IList<MailAddress> To
    {
      get
      {
        if (_messageInfo != null) return _messageInfo.Envelope.To;
        return _iMail != null ? _iMail.To : new List<MailAddress>();
      }
    }

    public DateTime? Date
    {
      get
      {
        if (_messageInfo != null) return _messageInfo.Envelope.Date;
        return _iMail != null ? _iMail.Date : null;
      }
    }

    public List<MimeObject> Attachments
    {
      get
      {
        if (_messageInfo != null) return _messageInfo.BodyStructure.Attachments.Select(mimeStructure => new MimeObject(mimeStructure)).ToList();
        return _iMail != null ? _iMail.Attachments.Select(mimeData => new MimeObject(mimeData)).ToList() : null;
      }
    }

    #endregion
  }

  public class MimeObject
  {
    #region Fields

    private readonly MimeStructure _mimeStructure;
    private readonly MimeData _mimeData;

    #endregion

    #region Constructors

    public MimeObject(MimeStructure mime)
    {
      _mimeStructure = mime;
    }

    public MimeObject(MimeData mime)
    {
      _mimeData = mime;
    }

    #endregion

    #region Properties

    public ContentType ContentType
    {
      get
      {
        if (_mimeStructure != null) return _mimeStructure.ContentType;
        return _mimeData != null ? _mimeData.ContentType : null;
      }
    }

    public string FileName
    {
      get
      {
        if (_mimeStructure != null) return _mimeStructure.FileName;
        return _mimeData != null ? _mimeData.FileName : String.Empty;
      }
    }

    public string SafeFileName
    {
      get
      {
        if (_mimeStructure != null) return _mimeStructure.SafeFileName;
        return _mimeData != null ? _mimeData.SafeFileName : String.Empty;
      }
    }

    public byte[] GetBytes()
    {
      return _mimeData != null ? _mimeData.Data : null;
    }

    public MimeStructure GetStructure()
    {
      return _mimeStructure;
    }

    #endregion
  }
}
