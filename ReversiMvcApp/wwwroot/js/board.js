let board = document.getElementById('board');
let baseUrl = null;
let token = null;
let spelerToken = null;
function start(_baseUrl, _token, _spelerToken) {
    console.log(_baseUrl, _token, _spelerToken);
    baseUrl = _baseUrl;
    token = _token;
    spelerToken = _spelerToken;
    
    getSpel();
    setInterval(getSpel,1000);
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