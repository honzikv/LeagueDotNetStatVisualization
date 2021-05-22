/**
 * Tato funkce se zavola tehdy, kdyz posleme AJAX na server pro ziskani match history
 * @param response XHR odpoved
 */
function OnLoadTimeline(response) {

    // Smazeme match timeline, aby uzivatel nemohl snadno na server nesmyslne posilat pozadavky vicekrat
    $('#MatchTimeline').remove();

    if (typeof response.StatusMessage === 'undefined') {
        return;
    }

    // Nyni mame k dispozici objekt MatchTimelineOverviewDto
    let matchTimeline = response.MatchTimeline;
    let playerDetail = response.PlayerDetail;
    let opponentParticipantId = matchTimeline.OpponentParticipantId;

    // Pokud by nahodou byl undefined vratime se, to by se ale stat nemelo.
    if (matchTimeline === undefined) {
        return;
    }

    // Nyni muzeme vytvorit grafy pro jednotlive zaznamy
    PopulateCharts(matchTimeline);
    PopulatePlayerDetail(playerDetail, opponentParticipantId);
    $('.ChampionTimeline').show();
}

/**
 *
 * @param timeline TimelineDto
 */
function PopulateCharts(timeline) {
    // Vytvorime slovnik pro kazdy graf, ktery bude drzet participantId a hodnoty pro graf
    let xpOverTime = {};
    let goldOverTime = {};
    let csOverTime = {};
    let levelOverTime = {};

    // Navic jeste udelame slovnik, pomoci ktereho ziskame summoner name z participantId
    let participantToSummonerName = {};

    timeline.ParticipantIds.forEach(participantId => {
        // PlayerTimelineDto objekt
        let playerTimeline = timeline.PlayerTimelines[participantId];

        participantToSummonerName[participantId] = playerTimeline.SummonerName;
        xpOverTime[participantId] = playerTimeline.XpOverTime;
        goldOverTime[participantId] = playerTimeline.GoldOverTime;
        csOverTime[participantId] = playerTimeline.CsOverTime;
        levelOverTime[participantId] = playerTimeline.LevelOverTime;
    });

    let labels = timeline.Intervals;
    // Mapper mappuje defaultne na string, ktery chceme pretypovat na cislo
    let playerParticipantId = timeline.PlayerParticipantId;
    let opponentParticipantId = timeline.OpponentParticipantId;

    // Nyni musime pro kazdy chart pridat data a nastavit onclick eventy pro ikonky aby data zobrazila
    // nebo deaktivovala
    CreateChart(xpOverTime, labels, participantToSummonerName, BackgroundColors, XpOverTimeChartId,
        playerParticipantId, opponentParticipantId);
    CreateChart(goldOverTime, labels, participantToSummonerName, BackgroundColors, GoldOverTimeChartId,
        playerParticipantId, opponentParticipantId);
    CreateChart(csOverTime, labels, participantToSummonerName, BackgroundColors, CsOverTimeChartId,
        playerParticipantId, opponentParticipantId);
    CreateChart(levelOverTime, labels, participantToSummonerName, BackgroundColors, LevelOverTimeChartId,
        playerParticipantId, opponentParticipantId);

}

function CreateChart(dataOverTime, labels, participantToSummonerName, backgroundColors, chartId,
                     playerParticipantId, opponentParticipantId) {
    let datasets = []

    // Nyni iterujeme pres slovnik (properties) s klicem participantId a hodnotou list<int>
    for (let property in dataOverTime) {
        if (!dataOverTime.hasOwnProperty(property)) {
            return;
        }

        // Pridame novy objekt s daty pro graf
        let participantId = property; // z nejakeho duvodu je key od 0 ?
        let series = dataOverTime[participantId];

        let show = Number(participantId) === playerParticipantId || Number(participantId) === opponentParticipantId;
        datasets.push({
            label: participantToSummonerName[participantId],
            backgroundColor: "rgba(0, 0, 0, 0)", // bez oblasti pod krivkou
            borderColor: backgroundColors[participantId - 1],
            pointBackgroundColor: backgroundColors[participantId - 1],
            data: series,
            hidden: !show
        });

    }


    // Ziskame kontext, id je definovano v globalni promenne
    let selector = "#" + chartId;
    let ctx = $(selector).get(0).getContext('2d');
    let chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: datasets
        },
        options: {}
    });

}

function PopulatePlayerDetail(playerDetailDto, opponentParticipantId) {
    Populate(playerDetailDto, 'MaxGoldDiff', 'max', 'gold');
    Populate(playerDetailDto, 'MinGoldDiff', 'min', 'gold');
    Populate(playerDetailDto, 'MaxCsDiff', 'max', 'cs');
    Populate(playerDetailDto, 'MinCsDiff', 'min', 'cs');
    Populate(playerDetailDto, 'MaxXpDiff', 'max', 'xp');
    Populate(playerDetailDto, 'MinXpDiff', 'min', 'xp');
    Populate(playerDetailDto, 'MaxLevelDiff', 'max', 'level');
    Populate(playerDetailDto, 'MinLevelDiff', 'min', 'level');

    // Zde je jedno co vybereme za property, protoze pokud existuje jedna budou existovat vsechny
    if (ShowDiv(playerDetailDto, 'CsDiffAt10', '10')) {
        PopulateAtTime(playerDetailDto, 'CsDiffAt10', 'cs-at', '10');
        PopulateAtTime(playerDetailDto, 'GoldDiffAt10', 'gold-at', '10');
        PopulateAtTime(playerDetailDto, 'LevelDiffAt10', 'level-at', '10');
        PopulateAtTime(playerDetailDto, 'XpDiffAt10', 'xp-at', '10');
    }

    // Zde je jedno co vybereme za property, protoze pokud existuje jedna budou existovat vsechny
    if (ShowDiv(playerDetailDto, 'CsDiffAt15', '15')) {
        PopulateAtTime(playerDetailDto, 'CsDiffAt15', 'cs-at', '15');
        PopulateAtTime(playerDetailDto, 'GoldDiffAt15', 'gold-at', '15');
        PopulateAtTime(playerDetailDto, 'LevelDiffAt15', 'level-at', '15');
        PopulateAtTime(playerDetailDto, 'XpDiffAt15', 'xp-at', '15');
    }
    

    // Zobrazime oponenta
    $('#pills-participant-' + opponentParticipantId).addClass("active");
    $('#pills-participant-' + opponentParticipantId + "-tab").tab('show');
    $('#pills-participant-' + opponentParticipantId + "-tab").attr("aria-selected", "true");
}

/**
 *
 * @param playerDetailDto dto objekt
 * @param propertyName jmeno property - MaxGoldDiff, MinGoldDiff apod.
 * @param type min nebo max
 * @param property gold, xp, level ...
 */
function Populate(playerDetailDto, propertyName, type, property) {
    for (let participantId in playerDetailDto[propertyName]) {
        if (!playerDetailDto[propertyName].hasOwnProperty(participantId)) {
            return;
        }
        let timeValue = playerDetailDto[propertyName][participantId];
        let value = timeValue.Value;
        let time = timeValue.Time;

        $(`#participant-${participantId}-${type}-${property}`).text(value);
        $(`#participant-${participantId}-${type}-${property}-time`).text("at " + time);

    }
}

function PopulateAtTime(playerDetailDto, propertyName, type, property) {
    for (let participantId in playerDetailDto[propertyName]) {
        if (playerDetailDto[propertyName].hasOwnProperty(participantId)) {
            let value = playerDetailDto[propertyName][participantId];
            $(`#participant-${participantId}-${type}-${property}`).text(value);
        }
    }
}

function ShowDiv(playerDetailDto, propertyName, property) {
    let shown = false;
    for (let participantId in playerDetailDto[propertyName]) {
        if (!playerDetailDto[propertyName].hasOwnProperty(propertyName)) {
            $('#participant-' + participantId + "-stats-at-" + property).removeClass("invisible");
            shown = true;
        }
    }

    return shown;
}
