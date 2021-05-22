/**
 * Tato funkce se zavola tehdy, kdyz posleme AJAX na server pro ziskani match history
 * @param response XHR odpoved
 */
function OnLoadTimeline(response) {

    // Smazeme match timeline, aby uzivatel nemohl snadno na server nesmyslne posilat pozadavky vicekrat
    $('#MatchTimeline').remove();

    console.log(response);

    // todo: vypsat error message
    if (typeof response.StatusMessage === 'undefined') {
        return;
    }

    // Nyni mame k dispozici objekt MatchTimelineOverviewDto
    let matchTimeline = response.MatchTimeline;

    // Pokud by nahodou byl undefined vratime se, to by se ale stat nemelo.
    if (matchTimeline === undefined) {
        return;
    }

    // Nyni muzeme vytvorit grafy pro jednotlive zaznamy
    PopulateCharts(matchTimeline);
}

// const XpOverTimeChartId = @ServerConstants.XpOverTimeChartId;
// const GoldOverTimeChartId = @ServerConstants.GoldOverTimeChartId;
// const CsOverTimeChartId = @ServerConstants.CsOverTimeChartId;
// const LevelOverTimeChartId = @ServerConstants.LevelOverTimeChartId;

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

    // Nyni musime pro kazdy chart pridat data a nastavit onclick eventy pro ikonky aby data zobrazila
    // nebo deaktivovala
    CreateChart(xpOverTime, labels, participantToSummonerName, BackgroundColors, XpOverTimeChartId);
    CreateChart(goldOverTime, labels, participantToSummonerName, BackgroundColors, GoldOverTimeChartId);
    CreateChart(csOverTime, labels, participantToSummonerName, BackgroundColors, CsOverTimeChartId);
    CreateChart(levelOverTime, labels, participantToSummonerName, BackgroundColors, LevelOverTimeChartId);

}

function CreateChart(dataOverTime, labels, participantToSummonerName, backgroundColors, chartId) {
    let datasets = []

    // Nyni iterujeme pres slovnik (properties) s klicem participantId a hodnotou list<int>
    for (const property in dataOverTime) {
        // Pridame novy objekt s daty pro graf
        let participantId = property; // z nejakeho duvodu je key od 0 ?
        let series = dataOverTime[participantId];
        datasets.push({
            label: participantToSummonerName[participantId],
            backgroundColor: backgroundColors[participantId - 1], // participantId zacina od 1
            borderColor: backgroundColors[participantId - 1],
            data: series
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
