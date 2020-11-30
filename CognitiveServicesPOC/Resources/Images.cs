using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServicesPOC.Resources
{
    public static class Images
    {
        public enum TestPerson
        {
            Tony,
            Peter,
            Steve
        }

        public static IEnumerable<byte[]> GetTrainFacesImages(TestPerson person)
        {
            IList<byte[]> imagesList = null;

            switch (person)
            {
                case TestPerson.Tony:
                    imagesList = new List<byte[]>()
                    {
                        CognitiveServicesPOC.Properties.Resources.tony_1,
                        CognitiveServicesPOC.Properties.Resources.tony_2,
                        CognitiveServicesPOC.Properties.Resources.tony_3,
                        CognitiveServicesPOC.Properties.Resources.tony_4
                    };
                    break;

                case TestPerson.Peter:
                    imagesList = new List<byte[]>()
                    {
                        CognitiveServicesPOC.Properties.Resources.peter_1,
                        CognitiveServicesPOC.Properties.Resources.peter_2,
                        CognitiveServicesPOC.Properties.Resources.peter_3,
                        CognitiveServicesPOC.Properties.Resources.peter_4
                    };
                    break;
            }

            return imagesList;
        }

        public static byte[] GetTestFaceImage(TestPerson person)
        {
            byte[] image = null;
            switch (person)
            {
                case TestPerson.Tony:
                    image = CognitiveServicesPOC.Properties.Resources.tony_5;
                    break;

                case TestPerson.Peter:
                    image = CognitiveServicesPOC.Properties.Resources.peter_5;
                    break;
                case TestPerson.Steve:
                    image = CognitiveServicesPOC.Properties.Resources.steve_1;
                    break;
            }
            return image;
        }
    }
}
