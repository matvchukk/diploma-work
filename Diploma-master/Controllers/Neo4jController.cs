using Diploma.Services;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;

namespace Diploma.Controllers
{
    [ApiController]
    [Route("api/neo4j")]
    public class Neo4jController : ControllerBase
    {
        private readonly Neo4jSyncService _neo4j;
        private readonly IDriver _driver;

        public Neo4jController(Neo4jSyncService neo4j, IDriver driver)
        {
            _neo4j = neo4j;
            _driver = driver;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync()
        {
            await _neo4j.SyncAllAsync();
            return Ok("Neo4j graph synchronized successfully");
        }

        [HttpPost("rebuild-graph")]
        public async Task<IActionResult> RebuildGraph()
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.RunAsync("CALL gds.graph.drop('reco_graph', false)");

                await session.RunAsync(@"
                    CALL gds.graph.project(
                      'reco_graph',
                      ['User', 'Product', 'Category', 'Seller', 'WishList'],
                      {
                        BOUGHT:       { type: 'BOUGHT', orientation: 'UNDIRECTED', properties: 'weight' },
                        REVIEWED:     { type: 'REVIEWED', orientation: 'UNDIRECTED', properties: 'weight' },
                        CONTAINS:     { type: 'CONTAINS', orientation: 'UNDIRECTED', properties: 'weight' },
                        HAS_WISHLIST: { type: 'HAS_WISHLIST', orientation: 'UNDIRECTED', properties: 'weight' },
                        IN_CATEGORY:  { type: 'IN_CATEGORY', orientation: 'UNDIRECTED', properties: 'weight' },
                        SOLD_BY:      { type: 'SOLD_BY', orientation: 'UNDIRECTED', properties: 'weight' }
                      }
                    )
                ");

                await session.RunAsync(@"
                    CALL gds.pageRank.write('reco_graph', {
                      writeProperty: 'pageRank',
                      relationshipWeightProperty: 'weight'
                    })
                ");

                await session.RunAsync(@"
                    CALL gds.fastRP.write('reco_graph', {
                      embeddingDimension: 128,
                      writeProperty: 'embedding',
                      relationshipWeightProperty: 'weight'
                    })
                ");

                await session.RunAsync(@"
                    CALL gds.beta.node2vec.write('reco_graph', {
                      writeProperty: 'node2vecEmbedding',
                      relationshipWeightProperty: 'weight'
                    })
                ");

                return Ok("Graph rebuilt and GDS algorithms executed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error rebuilding graph: {ex.Message}");
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}
