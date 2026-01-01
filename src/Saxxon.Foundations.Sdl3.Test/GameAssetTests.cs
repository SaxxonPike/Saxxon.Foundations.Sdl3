using Saxxon.Foundations.Sdl3.Game;

namespace Saxxon.Foundations.Sdl3.Test;

[TestFixture]
public class GameAssetTests
{
    public class TestGameWithAssetLoader : Game.Game
    {
        public Dictionary<string, object> Assets { get; set; } = [];
        public List<string> Requests { get; set; } = [];

        protected override bool OnAssetRequest<T>(string path, out T? asset) where T : class
        {
            var result = Assets.TryGetValue(path, out var obj);

            Assert.That(obj, Is.AssignableTo<T>());

            asset = (T?)obj;
            Requests.Add(path);
            return result;
        }
    }

    [Test]
    public void TestLoadAsset()
    {
        const string asset1Name = "asset1";
        const string asset2Name = "asset2";
        const string asset3Name = "asset3";

        const string asset1Content = "some game content";
        const string asset2Content = "some other game content";
        const string asset3Content = "dev asset that somehow made it into the final product";

        var game = new TestGameWithAssetLoader();
        Reset();

        using (Assert.EnterMultipleScope())
        {
            // Load an asset without caching, then load it again.

            var observedAsset1A = game.LoadAsset<string>(asset1Name, new AssetOptions
            {
                DoNotCache = true
            });
            Assert.That(observedAsset1A, Is.EqualTo(asset1Content));

            Assert.That(game.Requests, Has.Count.EqualTo(1));
            Assert.That(game.Requests[0], Is.EqualTo(asset1Name));

            var observedAsset1B = game.LoadAsset<string>(asset1Name);
            Assert.That(observedAsset1B, Is.EqualTo(asset1Content));

            Assert.That(game.Requests, Has.Count.EqualTo(2));
            Assert.That(game.Requests[1], Is.EqualTo(asset1Name));

            Reset();

            // Load an asset with caching, then load it again.

            observedAsset1A = game.LoadAsset<string>(asset1Name);
            Assert.That(observedAsset1A, Is.EqualTo(asset1Content));

            Assert.That(game.Requests, Has.Count.EqualTo(1));
            Assert.That(game.Requests[0], Is.EqualTo(asset1Name));

            observedAsset1B = game.LoadAsset<string>(asset1Name);
            Assert.That(observedAsset1B, Is.SameAs(observedAsset1A));

            Assert.That(game.Requests, Has.Count.EqualTo(1));

            Reset();

            // Force load the asset, then force load the asset once more.

            observedAsset1A = game.LoadAsset<string>(asset1Name, new AssetOptions
            {
                Force = true
            });
            Assert.That(observedAsset1A, Is.EqualTo(asset1Content));

            Assert.That(game.Requests, Has.Count.EqualTo(1));
            Assert.That(game.Requests[0], Is.EqualTo(asset1Name));

            observedAsset1B = game.LoadAsset<string>(asset1Name, new AssetOptions
            {
                Force = true
            });
            Assert.That(observedAsset1B, Is.EqualTo(asset1Content));

            Assert.That(game.Requests, Has.Count.EqualTo(2));
            Assert.That(game.Requests[1], Is.EqualTo(asset1Name));

            Reset();
        }

        return;

        void Reset()
        {
            game.Requests.Clear();
            game.ClearAssetCache();
            game.Assets = new Dictionary<string, object>
            {
                { asset1Name, asset1Content },
                { asset2Name, asset2Content },
                { asset3Name, asset3Content }
            };
        }
    }
}