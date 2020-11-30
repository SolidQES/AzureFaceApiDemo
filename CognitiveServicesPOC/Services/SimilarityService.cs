using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesPOC.Services
{
    public class SimilarityService : FaceAPI
    {
        private readonly string TestListId, TestListName;

        public SimilarityService(string endpoint, string apiKey, string listId, string listName) : base(endpoint, apiKey) 
        {
            TestListId = listId;
            TestListName = listName;

            // Creamos la lista de imágenes con un identificador
            FaceApiClient.LargeFaceList.CreateAsync(TestListId, name: TestListName).Wait();
        }

        public async Task TrainService(IEnumerable<byte[]> faceImages)
        {
            var faceList = await FaceApiClient.LargeFaceList.GetAsync(TestListId);

            foreach(var image in faceImages)
            {
                // Añadimos nueva imagen a la lista
                var faceId = await FaceApiClient.LargeFaceList.AddFaceFromStreamAsync(faceList.LargeFaceListId, new MemoryStream(image), userData: TestListName);
            }

            // Entrenamos el modelo con las nuevas imágenes
            await FaceApiClient.LargeFaceList.TrainAsync(TestListId);
        }

        public async Task<IList<SimilarFace>> GetSimilarities(byte[] newImage)
        {
            // Detectamos la cara dentro de la imagen, en este caso controlado nos quedamos sólo con la primera que se detecta
            Guid? detectedFace = (await FaceApiClient.Face.DetectWithStreamAsync(new MemoryStream(newImage))).Select(_ => _.FaceId.Value)?.First();

            // Devolvermos una lista con el grado de similitud entre la nueva imagen y todas la de la lista registrada en el modelo
            return await FaceApiClient.Face.FindSimilarAsync(detectedFace.Value, largeFaceListId: TestListId, mode: FindSimilarMatchMode.MatchFace);
        }

        public async Task DeletePersonFaceList()
        {
            await FaceApiClient.LargeFaceList.DeleteAsync(TestListId);
        }
    }
}
