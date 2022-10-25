using SophiApp.Dto;
using SophiApp.Models;
using System;
using System.Collections.Generic;

namespace SophiApp.Interfaces
{
    internal interface IParentElements
    {
        List<TextedElement> ChildElements { get; set; }

        List<TextedElementDto> ChildsDTO { get; set; }

        void OnChildErrorOccured(TextedElement child, Exception e);
    }
}