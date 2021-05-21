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
    let matchTimeline = response.matchTimeline;
    
    // Pokud by nahodou byl undefined vratime se, to by se ale stat nemelo.
    if (matchTimeline === undefined) {
        return;
    }
    
    // Nyni muzeme vytvorit grafy pro jednotlive zaznamy
    CreateCharts(matchTimeline);
}

function CreateCharts(matchTimeline) {
    let players = {};
    
    for (let player in matchTimeline.playerTimelines) {
        
    }
}

function CreateGoldPerFrameChart() {
    
}
