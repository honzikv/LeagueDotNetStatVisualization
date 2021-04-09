using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dotNetMVCLeagueApp.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerStatsModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Win = table.Column<bool>(type: "bit", nullable: false),
                    TotalUnitsHealed = table.Column<int>(type: "int", nullable: false),
                    Item0 = table.Column<int>(type: "int", nullable: false),
                    Item1 = table.Column<int>(type: "int", nullable: false),
                    Item2 = table.Column<int>(type: "int", nullable: false),
                    Item4 = table.Column<int>(type: "int", nullable: false),
                    Item3 = table.Column<int>(type: "int", nullable: false),
                    Item6 = table.Column<int>(type: "int", nullable: false),
                    Item5 = table.Column<int>(type: "int", nullable: false),
                    LargestMultiKill = table.Column<int>(type: "int", nullable: false),
                    GoldEarned = table.Column<int>(type: "int", nullable: false),
                    FirstInhibitorKill = table.Column<bool>(type: "bit", nullable: false),
                    PhysicalDamageTaken = table.Column<long>(type: "bigint", nullable: false),
                    TotalPlayerScore = table.Column<int>(type: "int", nullable: false),
                    ChampLevel = table.Column<int>(type: "int", nullable: false),
                    DamageDealtToObjectives = table.Column<long>(type: "bigint", nullable: false),
                    TotalDamageTaken = table.Column<long>(type: "bigint", nullable: false),
                    NeutralMinionsKilled = table.Column<int>(type: "int", nullable: false),
                    Deaths = table.Column<int>(type: "int", nullable: false),
                    TripleKills = table.Column<int>(type: "int", nullable: false),
                    MagicDamageDealtToChampions = table.Column<long>(type: "bigint", nullable: false),
                    WardsKilled = table.Column<int>(type: "int", nullable: false),
                    PentaKills = table.Column<int>(type: "int", nullable: false),
                    DamageSelfMitigated = table.Column<long>(type: "bigint", nullable: false),
                    LargestCriticalStrike = table.Column<int>(type: "int", nullable: false),
                    TotalTimeCrowdControlDealt = table.Column<int>(type: "int", nullable: false),
                    FirstTowerKill = table.Column<bool>(type: "bit", nullable: false),
                    MagicDamageDealt = table.Column<long>(type: "bigint", nullable: false),
                    WardsPlaced = table.Column<int>(type: "int", nullable: false),
                    TotalDamageDealt = table.Column<long>(type: "bigint", nullable: false),
                    TimeCCingOthers = table.Column<long>(type: "bigint", nullable: false),
                    MagicalDamageTaken = table.Column<long>(type: "bigint", nullable: false),
                    LargestKillingSpree = table.Column<int>(type: "int", nullable: false),
                    TotalDamageDealtToChampions = table.Column<long>(type: "bigint", nullable: false),
                    PhysicalDamageDealtToChampions = table.Column<long>(type: "bigint", nullable: false),
                    TotalMinionsKilled = table.Column<int>(type: "int", nullable: false),
                    Kills = table.Column<int>(type: "int", nullable: false),
                    CombatPlayerScore = table.Column<int>(type: "int", nullable: false),
                    InhibitorKills = table.Column<int>(type: "int", nullable: false),
                    TurretKills = table.Column<int>(type: "int", nullable: false),
                    ParticipantId = table.Column<int>(type: "int", nullable: false),
                    TrueDamageTaken = table.Column<long>(type: "bigint", nullable: false),
                    Assists = table.Column<int>(type: "int", nullable: false),
                    TeamObjective = table.Column<int>(type: "int", nullable: false),
                    TotalHeal = table.Column<long>(type: "bigint", nullable: false),
                    VisionScore = table.Column<long>(type: "bigint", nullable: false),
                    PhysicalDamageDealt = table.Column<long>(type: "bigint", nullable: false),
                    FirstBloodKill = table.Column<bool>(type: "bit", nullable: false),
                    KillingSprees = table.Column<int>(type: "int", nullable: false),
                    SightWardsBoughtInGame = table.Column<int>(type: "int", nullable: false),
                    TrueDamageDealtToChampions = table.Column<long>(type: "bigint", nullable: false),
                    DoubleKills = table.Column<int>(type: "int", nullable: false),
                    TrueDamageDealt = table.Column<long>(type: "bigint", nullable: false),
                    QuadraKills = table.Column<int>(type: "int", nullable: false),
                    Perk0 = table.Column<int>(type: "int", nullable: false),
                    Perk0Var1 = table.Column<int>(type: "int", nullable: false),
                    Perk0Var2 = table.Column<int>(type: "int", nullable: false),
                    Perk0Var3 = table.Column<int>(type: "int", nullable: false),
                    Perk1 = table.Column<int>(type: "int", nullable: false),
                    Perk1Var1 = table.Column<int>(type: "int", nullable: false),
                    Perk1Var2 = table.Column<int>(type: "int", nullable: false),
                    Perk1Var3 = table.Column<int>(type: "int", nullable: false),
                    Perk2 = table.Column<int>(type: "int", nullable: false),
                    Perk2Var1 = table.Column<int>(type: "int", nullable: false),
                    Perk2Var2 = table.Column<int>(type: "int", nullable: false),
                    Perk2Var3 = table.Column<int>(type: "int", nullable: false),
                    Perk3 = table.Column<int>(type: "int", nullable: false),
                    Perk3Var1 = table.Column<int>(type: "int", nullable: false),
                    Perk3Var2 = table.Column<int>(type: "int", nullable: false),
                    Perk3Var3 = table.Column<int>(type: "int", nullable: false),
                    Perk4 = table.Column<int>(type: "int", nullable: false),
                    Perk4Var1 = table.Column<int>(type: "int", nullable: false),
                    Perk4Var2 = table.Column<int>(type: "int", nullable: false),
                    Perk4Var3 = table.Column<int>(type: "int", nullable: false),
                    Perk5 = table.Column<int>(type: "int", nullable: false),
                    Perk5Var1 = table.Column<int>(type: "int", nullable: false),
                    Perk5Var2 = table.Column<int>(type: "int", nullable: false),
                    Perk5Var3 = table.Column<int>(type: "int", nullable: false),
                    PerkPrimaryStyle = table.Column<int>(type: "int", nullable: false),
                    PerkSubStyle = table.Column<int>(type: "int", nullable: false),
                    StatPerk0 = table.Column<int>(type: "int", nullable: false),
                    StatPerk1 = table.Column<int>(type: "int", nullable: false),
                    StatPerk2 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStatsModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SummonerInfoModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EncryptedSummonerId = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: true),
                    EncryptedAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SummonerLevel = table.Column<long>(type: "bigint", nullable: false),
                    ProfileIconId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummonerInfoModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MatchInfoModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SummonerInfoModelId = table.Column<int>(type: "int", nullable: true),
                    Win = table.Column<bool>(type: "bit", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    PlayTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchInfoModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchInfoModels_SummonerInfoModels_SummonerInfoModelId",
                        column: x => x.SummonerInfoModelId,
                        principalTable: "SummonerInfoModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QueueInfoModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QueueType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeaguePoints = table.Column<int>(type: "int", nullable: false),
                    Wins = table.Column<int>(type: "int", nullable: false),
                    Losses = table.Column<int>(type: "int", nullable: false),
                    SummonerInfoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueInfoModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueInfoModels_SummonerInfoModels_SummonerInfoId",
                        column: x => x.SummonerInfoId,
                        principalTable: "SummonerInfoModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerInfoModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchInfoModelId = table.Column<int>(type: "int", nullable: false),
                    PlayerStatsModelId = table.Column<int>(type: "int", nullable: true),
                    SummonerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SummonerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileIcon = table.Column<int>(type: "int", nullable: false),
                    Spell1Id = table.Column<int>(type: "int", nullable: false),
                    Spell2Id = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lane = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CsPerMinute = table.Column<double>(type: "float", nullable: true),
                    GoldDiffAt10 = table.Column<double>(type: "float", nullable: true),
                    CsDiffAt10 = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerInfoModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerInfoModels_MatchInfoModels_MatchInfoModelId",
                        column: x => x.MatchInfoModelId,
                        principalTable: "MatchInfoModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerInfoModels_PlayerStatsModel_PlayerStatsModelId",
                        column: x => x.PlayerStatsModelId,
                        principalTable: "PlayerStatsModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamStatsInfoModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchInfoId = table.Column<int>(type: "int", nullable: true),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    Win = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TowerKills = table.Column<int>(type: "int", nullable: false),
                    RiftHeraldKills = table.Column<int>(type: "int", nullable: false),
                    FirstBlood = table.Column<bool>(type: "bit", nullable: false),
                    InhibitorKills = table.Column<int>(type: "int", nullable: false),
                    FirstBaron = table.Column<bool>(type: "bit", nullable: false),
                    FirstDragon = table.Column<bool>(type: "bit", nullable: false),
                    DragonKills = table.Column<int>(type: "int", nullable: false),
                    BaronKills = table.Column<int>(type: "int", nullable: false),
                    FirstInhibitor = table.Column<bool>(type: "bit", nullable: false),
                    FirstTower = table.Column<bool>(type: "bit", nullable: false),
                    FirstRiftHerald = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamStatsInfoModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamStatsInfoModels_MatchInfoModels_MatchInfoId",
                        column: x => x.MatchInfoId,
                        principalTable: "MatchInfoModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChampionBanModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChampionId = table.Column<int>(type: "int", nullable: false),
                    PickTurn = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    TeamStatsInfoModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChampionBanModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChampionBanModels_TeamStatsInfoModels_TeamStatsInfoModelId",
                        column: x => x.TeamStatsInfoModelId,
                        principalTable: "TeamStatsInfoModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChampionBanModels_TeamStatsInfoModelId",
                table: "ChampionBanModels",
                column: "TeamStatsInfoModelId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchInfoModels_SummonerInfoModelId",
                table: "MatchInfoModels",
                column: "SummonerInfoModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerInfoModels_MatchInfoModelId",
                table: "PlayerInfoModels",
                column: "MatchInfoModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerInfoModels_PlayerStatsModelId",
                table: "PlayerInfoModels",
                column: "PlayerStatsModelId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueInfoModels_SummonerInfoId",
                table: "QueueInfoModels",
                column: "SummonerInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamStatsInfoModels_MatchInfoId",
                table: "TeamStatsInfoModels",
                column: "MatchInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChampionBanModels");

            migrationBuilder.DropTable(
                name: "PlayerInfoModels");

            migrationBuilder.DropTable(
                name: "QueueInfoModels");

            migrationBuilder.DropTable(
                name: "TeamStatsInfoModels");

            migrationBuilder.DropTable(
                name: "PlayerStatsModel");

            migrationBuilder.DropTable(
                name: "MatchInfoModels");

            migrationBuilder.DropTable(
                name: "SummonerInfoModels");
        }
    }
}
