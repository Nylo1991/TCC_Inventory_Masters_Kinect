using Google.Cloud.Firestore;
using InventoryMaster.Models;
using InventoryMasters.Services;

namespace InventoryMasters.Repositories;

public class UsuarioRepository
{
    private readonly FirestoreDb _db;

    public UsuarioRepository(FirebaseService firebase)
    {
        _db = firebase.FirestoreDb;
    }

    public async Task AdicionarUsuario(Usuario usuario)
    {
        CollectionReference usuarios =
            _db.Collection("usuarios");

        await usuarios.AddAsync(usuario);
    }

    public async Task<List<Usuario>> ObterUsuarios()
    {
        QuerySnapshot snapshot =
            await _db.Collection("usuarios")
                     .GetSnapshotAsync();

        return snapshot.Documents
            .Select(doc => doc.ConvertTo<Usuario>())
            .ToList();
    }
}