using Bcat;
using SmashUltimate.Bcat;
using System;
using System.Collections.Generic;

namespace SmashBcatDetector.Json.S3
{
    public class StrippedContainer
    {
        public string Id
        {
            get;
            set;
        }

        public Dictionary<Language, string> Text
        {
            get;
            set;
        }

        public static StrippedContainer ConvertToStrippedContainer(SmashUltimate.Bcat.Container container)
        {
            // Create a new StrippedContainer
            StrippedContainer strippedContainer = new StrippedContainer();

            // Copy the ID
            strippedContainer.Id = container.Id;

            // Copy the correct text based on type
            if (container is Event)
            {
                strippedContainer.Text = ((Event)container).TitleText;
            }
            else if (container is LineNews)
            {
                strippedContainer.Text = ((LineNews)container).OneLines[0].Text;
            }
            else if (container is PopUpNews)
            {
                strippedContainer.Text = ((PopUpNews)container).TitleText;
            }
            else if (container is Present)
            {
                strippedContainer.Text = ((Present)container).TitleText;
            }
            else
            {
                throw new Exception("Unsupported container type");
            }

            // Return the final instance
            return strippedContainer;
        }

    }
}