let board = document.getElementById('board');
let baseUrl = null;
let token = null;
let spelerToken = null;
let chart1 = null;
let chart2 = null;

function initChart() {
    chart1 = new Chart(document.getElementById('myChart').getContext('2d'), {
        type: 'line',
        options: {
            animation: {
                duration: 0,
            },
            legend: {
                display: true,
                position: 'top',
                labels: {
                    boxWidth: 80,
                    fontColor: 'black'
                }
            }
        }
    });
    
    chart2 = new Chart(document.getElementById('myChart2').getContext('2d'), {
        type: 'line',
        options: {
            animation: {
                duration: 0,
            },
            legend: {
                display: true,
                position: 'top',
                labels: {
                    boxWidth: 80,
                    fontColor: 'black'
                }
            }
        }
    });
}

function start(_baseUrl, _token, _spelerToken) {
    console.log(_baseUrl, _token, _spelerToken);
    baseUrl = _baseUrl;
    token = _token;
    spelerToken = _spelerToken;
    
    initChart();
    getSpel();
    statistieken();
    setInterval(getSpel,1000);
    setInterval(statistieken,1000);
}

async function getSpel() {
    let formData = new FormData();
    formData.append('id', token);
    
    axios({
        method: "post",
        url: baseUrl + '/spel/afgelopen',
        data: formData,
        headers: { "Content-Type": "multipart/form-data" },
    }).then(r => {
        console.log(r.data);
        if (r.data.status == 605) {
            showMessage("Het spel is afgelopen", 'success');

            window.location.href = '/spel/done?token='+ token +'&spelerToken=' + spelerToken;
        }
    }).catch(e => console.log(e));
    
    axios.get(baseUrl + '/spel/krijg-spel?token=' + token)
        .then(response => {
            console.log(response.data);
            board.innerHTML = '';
            spel = response.data;
            let body = '';
            let clickHandlers = [];
            for ([index, player] of Object.entries(spel.bord)) {
                index = index.split(',');

                if (index[1] == '0') {
                    body += `<div class="c-row-f">`;
                }

                let fiche = '';
                if (player == 1) {
                    fiche = '<div class="c-f-white"></div>';
                }

                if (player == 2) {
                    fiche = '<div class="c-f-black"></div>';
                }
                
                if (
                    (spel.speler1Token == spelerToken && spel.aandeBeurt == 2) ||
                    (spel.speler2Token == spelerToken && spel.aandeBeurt == 1)
                ) {
                    document.getElementById("turn").innerText = "Jij bent aan de beurt";
                } else {
                    document.getElementById("turn").innerText = "Tegenstander is aan de beurt";
                }
                

                let uniqueId = `click-${index[0]}-${index[1]}`;

                body += `<div id="${uniqueId}" class="c-f">${fiche}</div>`;

                clickHandlers.push({'id': spel.token, 'token': spelerToken, 'rij': index[0], 'kolom': index[1], 'handlerId': uniqueId});

                if (index[1] == '7') {
                    body += '</div>';
                }
            }
            
            board.innerHTML += body;

            clickHandlers.forEach((e) => document.getElementById(e.handlerId).addEventListener("click", () => click(e.id, e.token, e.rij, e.kolom)));
        })
        .catch(e => console.log(e));
}

function click(id, token, rij, kolom) {
    console.log(rij, kolom);
    let formData = new FormData();
    formData.append('id', id);
    formData.append('token', token);
    formData.append('rij', rij);
    formData.append('kolom', kolom);

    axios({
        method: "post",
        url: baseUrl + '/spel/doe-zet',
        data: formData,
        headers: { "Content-Type": "multipart/form-data" },
    })
        .then(e => {
            if (e.data.status == 602) {
                showMessage('Deze zet is niet mogelijk', 'error');
            } else if (e.data.status == 601) {
                showMessage('Het is niet jouw beurt', 'error');
            } else if (e.data.status == 200) {
                getSpel();
            } else if (e.data.status == 605) {
                showMessage("Het spel is afgelopen", 'success');

                window.location.href = '/spel/done?token='+ token +'&spelerToken=' + spelerToken;
            }
            else {
                showMessage('Er is iets mis gegaan', 'error');
            }
        })
        .catch(e => {
            showMessage('Er is iets mis gegaan', 'error');
        });
}

function beurtOverslaan() {
    let formData = new FormData();
    formData.append('id', token);
    formData.append('token', spelerToken);

    axios({
        method: "post",
        url: baseUrl + '/spel/overslaan',
        data: formData,
        headers: { "Content-Type": "multipart/form-data" },
    })
        .then(e => {
            console.log(e.data);
            if (e.data.status == 606) {
                showMessage('Je kan nog een set doen', 'error');
            } else if (e.data.status == 605) {
                showMessage("Het spel is afgelopen", 'success');

                window.location.href = '/spel/done?token='+ token +'&spelerToken=' + spelerToken;
            } else {
                getSpel();
            }
        })
        .catch(e => {
            showMessage('Er is iets mis gegaan', 'error');
        });
}

function showMessage(message, type) {
    let msg = document.getElementById('message');

    if (type == 'success') {
        msg.classList.add('c-m-success');
    } else {
        msg.classList.add('c-m-error');
    }

    msg.innerText = message;
    msg.style.display = 'block';
    setTimeout(function() {
        msg.style.display = 'none';
    }, 5000);
}

function statistieken() {
    axios.get(baseUrl + '/spel/statistieken?token=' + token).then(response => {
        console.log(response.data.length, response.data);

        let datums = [];
        let geen = [];
        let zwart = [];
        let wit = [];
        
        for (let i = 0; i < response.data.length; i++) {
            console.log("DATUM: " + response.data[i][0].datum);
            datums.push(response.data[i][0].datum);
            
            for (let j = 0; j < response.data[i].length; j++) {
                console.log("MEU");
                if (response.data[i][j].type == 'aantal-wit') {
                    wit.push(response.data[i][j].waarde);   
                } else if (response.data[i][j].type == 'aantal-zwart') {
                    zwart.push(response.data[i][j].waarde);    
                } else {
                    geen.push(response.data[i][j].waarde);
                }
            }
        }
        
        // destroy chart
        
        
        var dataFirst = {
            label: "Zwart",
            data: zwart,
            lineTension: 0,
            fill: false,
            borderColor: 'black'
        };

        var dataSecond = {
            label: "Wit",
            data: wit,
            lineTension: 0,
            fill: false,
            borderColor: 'blue'
        };

        var dataThird = {
            label: "Geen",
            data: geen,
            lineTension: 0,
            fill: false,
            borderColor: 'red'
        };
        
        // new Chart(document.getElementById('myChart').getContext('2d'), {
        //     type: 'line',
        //     data: {
        //         labels: datums,
        //         datasets: [dataFirst, dataSecond]
        //     },
        //     options: {
        //         legend: {
        //             display: true,
        //             position: 'top',
        //             labels: {
        //                 boxWidth: 80,
        //                 fontColor: 'black'
        //             }
        //         }
        //     }
        // }).update().resize(250, 250);

        // new Chart(document.getElementById('myChart2').getContext('2d'), {
        //     type: 'line',
        //     data: {
        //         labels: datums,
        //         datasets: [dataThird]
        //     },
        //     options: {
        //         legend: {
        //             display: true,
        //             position: 'top',
        //             labels: {
        //                 boxWidth: 80,
        //                 fontColor: 'black'
        //             }
        //         }
        //     }
        // }).update().resize(250, 250);

        chart1.data = {
            labels: datums,
            datasets: [dataFirst, dataSecond, dataThird]
        };
        chart1.update();
        
        chart2.data = {
            labels: datums,
            datasets: [dataThird]
        };
        chart2.update();

    });
}