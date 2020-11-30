using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CognitiveServicesPOC.Resources.Images;

namespace CognitiveServicesPOC.Services
{
    public class RecognitionService : FaceAPI
    {
        private readonly string TestGroupId;
        private readonly PersonGroup TestGroup;

        public RecognitionService(string endpoint, string apiKey, string groupId) : base(endpoint, apiKey) 
        {
            PersonGroup group = null;
            var groupList = FaceApiClient.PersonGroup.ListAsync(); groupList.Wait();

            group = groupList.Result.FirstOrDefault(_ => _.PersonGroupId == groupId);
            if (group == null)
            {
                FaceApiClient.PersonGroup.CreateAsync(groupId, "TestGroup").Wait();

                groupList = FaceApiClient.PersonGroup.ListAsync(); groupList.Wait();
                group = groupList.Result.FirstOrDefault(_ => _.PersonGroupId == groupId);
            }

            TestGroupId = groupId;
            TestGroup = group;
        }
        
        public async Task TrainService(IEnumerable<byte[]> faceImages, string personId)
        {
            // Obtenemos/creamos a la persona en el grupo actual
            Person person = (await FaceApiClient.PersonGroupPerson.ListAsync(TestGroupId)).FirstOrDefault(_ => _.Name == personId);
            
            if(person == null)
                person = await FaceApiClient.PersonGroupPerson.CreateAsync(TestGroupId, name: personId);

            // Cargamos las imágenes de la persona al modelo
            foreach (var face in faceImages)
            {
                await FaceApiClient.PersonGroupPerson.AddFaceFromStreamAsync(TestGroupId, person.PersonId, new MemoryStream(face));
            }

            // Entrenamos el modelo
            await FaceApiClient.PersonGroup.TrainAsync(TestGroupId);
        }

        public async Task<string> RecognizeInGroup(byte[] newFaceImage)
        {
            Person recognizedPerson = null;

            // Detectamos la cara dentro de la nueva imagen y la identificamos dentro del grupo actual
            var facesInImage = (await FaceApiClient.Face.DetectWithStreamAsync(new MemoryStream(newFaceImage))).Select(_ => _.FaceId).ToList();
            var identifiedFace = (await FaceApiClient.Face.IdentifyAsync(facesInImage, personGroupId: TestGroup.PersonGroupId)).FirstOrDefault();

            // La cara detectada puede tener varios candidatos, cada uno de ellos presenta un grado de confianza
            if (identifiedFace?.Candidates?.Count() > 0)
            {
                // Nos quedamos con el que muestra la mayor confianza
                var candidate = identifiedFace.Candidates.OrderByDescending(_ => _.Confidence).FirstOrDefault(); 
                recognizedPerson = await FaceApiClient.PersonGroupPerson.GetAsync(TestGroupId, candidate.PersonId);
            }

            return recognizedPerson?.Name;
        }

        public async Task CleanGroup()
        {
            await FaceApiClient.PersonGroup.DeleteAsync(TestGroupId);
        }
    }
}
