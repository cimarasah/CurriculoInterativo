// Configurações da API
const API_BASE_URL = "http://localhost:5083/api";
const AZURE_FUNCTION_URL =
  "https://your-azure-function.azurewebsites.net/api/CalcularProficiencia";

// Estado da aplicação
let skillsUpdateInterval;

// Inicialização
document.addEventListener("DOMContentLoaded", function () {
  initializeApp();
});

function initializeApp() {
  console.log("Inicializando aplicação...");
  loadSkillsCounter();
  startSkillsUpdateTimer();
}

// Função para alternar visibilidade dos endpoints
function toggleEndpoint(endpointId) {
  const endpointGroup = document.querySelector(`#${endpointId}`).parentElement;
  endpointGroup.classList.toggle("active");
}

// Função para executar endpoints da API
async function executeEndpoint(endpoint) {
  const resultContainer = document.getElementById(`${endpoint}-result`);
  resultContainer.classList.add("show");

  try {
    showLoading(resultContainer);

    const response = await fetch(`${API_BASE_URL}/${endpoint}`);

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }

    const data = await response.json();
    showResult(resultContainer, data, "success");
  } catch (error) {
    console.error(`Erro ao executar endpoint ${endpoint}:`, error);
    showResult(resultContainer, { error: error.message }, "error");
  }
}

// Função para carregar o contador de skills
async function loadSkillsCounter() {
  const skillsGrid = document.getElementById("skills-grid");

  try {
    // Primeiro, buscar as skills da API local
   // const skillsResponse = await fetch(`${API_BASE_URL}/skill`);

   // if (!skillsResponse.ok) {
   //   throw new Error("Erro ao carregar skills da API local");
   // }

   // const skills = await skillsResponse.json();

    // Depois, buscar os tempos calculados da Azure Function
   // await updateSkillsTimes(skills);
  } catch (error) {
    console.error("Erro ao carregar contador de skills:", error);
    showSkillsError();
  }
}

// Função para atualizar os tempos das skills
async function updateSkillsTimes(skills) {
  const skillsGrid = document.getElementById("skills-grid");

  try {
    // Simular chamada para Azure Function (já que não temos uma real rodando)
    const skillsWithTimes = await simulateAzureFunctionCall(skills);

    // Limpar grid atual
    skillsGrid.innerHTML = "";

    // Renderizar skills com tempos atualizados
    skillsWithTimes.forEach((skill) => {
      const skillCard = createSkillCard(skill);
      skillsGrid.appendChild(skillCard);
    });
  } catch (error) {
    console.error("Erro ao atualizar tempos das skills:", error);
  }
}

// Simulação da Azure Function (para demonstração)
async function simulateAzureFunctionCall(skills) {
  return skills.map((skill) => {
    const dataInicio = new Date(skill.dataInicio);
    const agora = new Date();
    const tempo = calculateTimeDifference(dataInicio, agora);

    return {
      ...skill,
      tempoExperiencia: tempo,
    };
  });
}

// Função para calcular diferença de tempo
function calculateTimeDifference(startDate, endDate) {
  const diff = endDate - startDate;

  const anos = Math.floor(diff / (1000 * 60 * 60 * 24 * 365));
  const meses = Math.floor(
    (diff % (1000 * 60 * 60 * 24 * 365)) / (1000 * 60 * 60 * 24 * 30)
  );
  const dias = Math.floor(
    (diff % (1000 * 60 * 60 * 24 * 30)) / (1000 * 60 * 60 * 24)
  );
  const horas = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
  const minutos = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
  const segundos = Math.floor((diff % (1000 * 60)) / 1000);

  return {
    anos,
    meses,
    dias,
    horas,
    minutos,
    segundos,
    tempoFormatado: formatTime(anos, meses, dias, horas, minutos, segundos),
  };
}

// Função para formatar tempo
function formatTime(anos, meses, dias, horas, minutos, segundos) {
  const partes = [];

  if (anos > 0) partes.push(`${anos} ${anos === 1 ? "ano" : "anos"}`);
  if (meses > 0) partes.push(`${meses} ${meses === 1 ? "mês" : "meses"}`);
  if (dias > 0) partes.push(`${dias} ${dias === 1 ? "dia" : "dias"}`);
  if (horas > 0) partes.push(`${horas} ${horas === 1 ? "hora" : "horas"}`);
  if (minutos > 0)
    partes.push(`${minutos} ${minutos === 1 ? "minuto" : "minutos"}`);
  partes.push(`${segundos} ${segundos === 1 ? "segundo" : "segundos"}`);

  return partes.join(", ");
}

// Função para criar card de skill
function createSkillCard(skill) {
  const card = document.createElement("div");
  card.className = "skill-card";

  card.innerHTML = `
        <div class="skill-name">${skill.nome}</div>
        <div class="skill-time">${skill.tempoExperiencia.tempoFormatado}</div>
        <div class="skill-category">${skill.categoria}</div>
    `;

  return card;
}

// Função para iniciar timer de atualização das skills
function startSkillsUpdateTimer() {
  // Atualizar a cada segundo
  skillsUpdateInterval = setInterval(() => {
    loadSkillsCounter();
  }, 1000);
}

// Função para mostrar loading
function showLoading(container) {
  container.innerHTML = '<div class="loading">Carregando...</div>';
}

// Função para mostrar resultado
function showResult(container, data, type) {
  const jsonString = JSON.stringify(data, null, 2);
  container.innerHTML = `<pre class="${type}">${jsonString}</pre>`;
}

// Função para mostrar erro nas skills
function showSkillsError() {
  const skillsGrid = document.getElementById("skills-grid");
  skillsGrid.innerHTML = `
        <div class="skill-card" style="background: #e74c3c;">
            <div class="skill-name">Erro ao carregar skills</div>
            <div class="skill-time">Verifique se a API está rodando</div>
            <div class="skill-category">Erro</div>
        </div>
    `;
}

// Função para parar timer (útil para cleanup)
function stopSkillsUpdateTimer() {
  if (skillsUpdateInterval) {
    clearInterval(skillsUpdateInterval);
  }
}

// Cleanup quando a página é fechada
window.addEventListener("beforeunload", function () {
  stopSkillsUpdateTimer();
});

// Função para testar conectividade com a API
async function testApiConnection() {
  try {
    const response = await fetch(`${API_BASE_URL}/Contact`);
    if (response.ok) {
      console.log("✅ API está respondendo");
      return true;
    } else {
      console.log("❌ API retornou erro:", response.status);
      return false;
    }
  } catch (error) {
    console.log("❌ Erro ao conectar com a API:", error.message);
    return false;
  }
}

// Executar teste de conectividade na inicialização
testApiConnection().then((isConnected) => {
  if (!isConnected) {
    console.log(
      "💡 Dica: Certifique-se de que a API .NET Core está rodando em http://localhost:5000"
    );
  }
});

// Adicionar funcionalidade de busca (bonus)
//function addSearchFunctionality() {
//  const searchInput = document.createElement("input");
//  searchInput.type = "text";
//  searchInput.placeholder = "Buscar endpoints...";
//  searchInput.className = "search-input";

//  searchInput.addEventListener("input", function (e) {
//    const searchTerm = e.target.value.toLowerCase();
//    const endpointGroups = document.querySelectorAll(".endpoint-group");

//    endpointGroups.forEach((group) => {
//      const path = group.querySelector(".path").textContent.toLowerCase();
//      const description = group
//        .querySelector(".description")
//        .textContent.toLowerCase();

//      if (path.includes(searchTerm) || description.includes(searchTerm)) {
//        group.style.display = "block";
//      } else {
//        group.style.display = "none";
//      }
//    });
//  });

//  const endpointsSection = document.querySelector(".endpoints");
//  const endpointsTitle = endpointsSection.querySelector("h2");
//  endpointsTitle.after(searchInput);
//}

function addSearchFunctionality() {
    // Criar container do input
    const searchContainer = document.createElement("div");
    searchContainer.className = "search-container";

    // Criar input de busca
    const searchInput = document.createElement("input");
    searchInput.type = "text";
    searchInput.placeholder = "Buscar endpoints...";
    searchInput.className = "search-input";

    // Criar ícone de lupa (Font Awesome)
    const searchIcon = document.createElement("i");
    searchIcon.className = "fas fa-search"; // use Font Awesome

    // Adicionar input e ícone ao container
    searchContainer.appendChild(searchInput);
    searchContainer.appendChild(searchIcon);

    // Inserir container após o título da seção de endpoints
    const endpointsSection = document.querySelector(".endpoints");
    const endpointsTitle = endpointsSection.querySelector("h2");
    endpointsTitle.after(searchContainer);

    // Função de filtragem dos endpoints
    searchInput.addEventListener("input", function (e) {
        const searchTerm = e.target.value.toLowerCase();
        const endpointGroups = document.querySelectorAll(".endpoint-group");

        endpointGroups.forEach((group) => {
            const path = group.querySelector(".path").textContent.toLowerCase();
            const description = group
                .querySelector(".description")
                .textContent.toLowerCase();

            if (path.includes(searchTerm) || description.includes(searchTerm)) {
                group.style.display = "block";
            } else {
                group.style.display = "none";
            }
        });
    });
}


// Adicionar funcionalidade de busca após carregamento
document.addEventListener("DOMContentLoaded", function () {
  setTimeout(addSearchFunctionality, 1000);
});
