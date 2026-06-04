// Detectar automaticamente a URL da API baseado no ambiente
const API_BASE_URL = (() => {
    // Se estiver em produção (Azure), usa a mesma origem
    if (window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1') {
        return window.location.origin + '/api';
    }
    // Se estiver em desenvolvimento local
    return 'http://localhost:5083/api';
})();

// Inicialização
document.addEventListener('DOMContentLoaded', function () {
    loadDashboard();
    setupFilterListeners();
    checkAuthStatus();
    // Processar callback do Google ao carregar a página
    handleGoogleCallback();
});

async function loadDashboard() {
    try {
        await Promise.all([
            loadCompanyLogos(),
            loadExperiencesTimeline(),
            loadAllSkillsForFilter(),
            loadAllProjects(), // Já carrega todos os projetos inicialmente
            loadSkills(),
            loadCertifications()
        ]);
    } catch (error) {
        console.error('Erro ao carregar dashboard:', error);
    }
}

//  Carregar Logos das Empresas
async function loadCompanyLogos() {
    try {
        const response = await fetch(`${API_BASE_URL}/experience`);
        if (!response.ok) throw new Error(`Erro de rede: ${response.status}`);

        const experiences = await response.json();
        const logoCollection = document.getElementById('company-logo-collection');
        if (!logoCollection) return;

        logoCollection.innerHTML = '';
        experiences.sort((a, b) => new Date(b.startDate) - new Date(a.startDate));

        experiences.forEach(exp => {
            const logoItem = document.createElement('div');
            logoItem.className = 'company-logo-item';
            logoItem.setAttribute('title', exp.company);

            if (exp.imgLogo) {
                const imgBase64Src = `data:image/jpeg;base64,${exp.imgLogo}`;
                logoItem.innerHTML = `
                    <img src="${imgBase64Src}" alt="${exp.company} Logo" class="logo-image">
                    <span class="logo-name">${exp.company}</span>
                `;
            } else {
                const initials = exp.company.split(' ').map(word => word[0]).join('').toUpperCase().slice(0, 2);
                logoItem.innerHTML = `
                    <div class="logo-initials-fallback">${initials}</div>
                    <span class="logo-name">${exp.company}</span>
                `;
            }
            logoCollection.appendChild(logoItem);
        });
    } catch (error) {
        console.error('Erro ao carregar logos das empresas:', error);
    }
}

// Timeline de Experiências (Jornada Profissional - Zig-Zag)
async function loadExperiencesTimeline() {
    try {
        const response = await fetch(`${API_BASE_URL}/experience`);
        if (!response.ok) throw new Error(`Erro de rede: ${response.status}`);

        const experiences = await response.json();
        const container = document.getElementById('timeline');
        container.innerHTML = '';

        // Ordenar do mais recente ao mais antigo
        const sorted = experiences.sort((a, b) => new Date(b.startDate) - new Date(a.startDate));

        sorted.forEach((exp, index) => {
            const startDate = new Date(exp.startDate);
            const endDate = exp.endDate ? new Date(exp.endDate) : new Date();
            const duration = calculateDuration(startDate, endDate);
            const isCurrent = !exp.endDate;
            const side = index % 2 === 0 ? 'left' : 'right';

            const item = document.createElement('div');
            item.className = `timeline-item ${side}`;
            item.innerHTML = `
                <span class="timeline-dot"></span>
                <div class="timeline-card">
                    ${isCurrent ? '<span class="timeline-current-badge">Atual</span>' : ''}
                    <div class="timeline-company">${exp.company}</div>
                    <div class="timeline-period">${startDate.getFullYear()} - ${isCurrent ? 'Presente' : endDate.getFullYear()}</div>
                    <div class="timeline-duration">${duration}</div>
                    <div class="timeline-period">${exp.description}</div>

                </div>
            `;
            container.appendChild(item);
        });
    } catch (error) {
        console.error('Erro ao carregar timeline de experiências:', error);
    }
}


function createTimelineCard(exp, container, isCurrent) {
    const startDate = new Date(exp.startDate);
    const endDate = exp.endDate ? new Date(exp.endDate) : new Date();
    const duration = calculateDuration(startDate, endDate);
    const isCurrentJob = !exp.endDate || isCurrent;

    // Formatar período
    const startYear = startDate.getFullYear();
    const endYear = isCurrentJob ? 'Presente' : endDate.getFullYear();
    const period = `${startYear} - ${endYear}`;

    const card = document.createElement('div');
    card.className = 'timeline-card';
    card.innerHTML = `
        ${isCurrentJob ? '<span class="timeline-current-badge">Atual</span>' : ''}
        <div class="timeline-card-content">
            <div class="timeline-company">${exp.company}</div>
            <div class="timeline-period">${period}</div>
            <div class="timeline-duration">${duration}</div>
        </div>
        <div class="timeline-year">${startYear}</div>
    `;

    container.appendChild(card);
}

//  Carregar Skills para o Filtro
async function loadAllSkillsForFilter() {
    try {
        const response = await fetch(`${API_BASE_URL}/skill`);
        if (!response.ok) throw new Error(`Erro de rede: ${response.status}`);

        const skills = await response.json();
        const selectElement = document.getElementById('skill-filter');
        if (!selectElement) return;

        selectElement.innerHTML = '<option value="">Exibir todos os projetos</option>';
        skills.sort((a, b) => a.name.localeCompare(b.name));

        skills.forEach(skill => {
            const option = document.createElement('option');
            option.value = skill.id;
            option.textContent = skill.name;
            selectElement.appendChild(option);
        });
    } catch (error) {
        console.error('Erro ao carregar skills para o filtro:', error);
    }
}

//  Configurar listeners do filtro
function setupFilterListeners() {
    const skillFilter = document.getElementById('skill-filter');
    if (!skillFilter) return;

    skillFilter.addEventListener('change', function () {
        const skillId = this.value;
        const filterInfo = document.getElementById('filter-info');

        if (skillId) {
            loadProjectsBySkill(parseInt(skillId));
        } else {
            filterInfo.style.display = 'none';
            loadAllProjects();
        }
    });
}

//  Carregar Projetos Filtrados por Skill
async function loadProjectsBySkill(skillId) {
    try {
        console.log(`Carregando projetos filtrados pela skill ID: ${skillId}`);
        const response = await fetch(`${API_BASE_URL}/project/${skillId}`);
        if (!response.ok) throw new Error(`Erro na API: ${response.status}`);

        const data = await response.json();
        console.log('Dados recebidos:', data);

        const container = document.getElementById('projects-container');
        const filterInfo = document.getElementById('filter-info');

        container.innerHTML = '';
        filterInfo.style.display = 'block';

        const experienceText = data.years > 0
            ? `${data.years} ano${data.years > 1 ? 's' : ''}${data.months > 0 ? ` e ${data.months} ${data.months > 1 ? 'meses' : 'mês'}` : ''}`
            : `${data.months} ${data.months > 1 ? 'meses' : 'mês'}`;

        filterInfo.innerHTML = `
            <div class="filter-info-content">
                <div class="filter-stat">
                    <i class="fas fa-code"></i>
                    <span>Tecnologia: <strong>${data.skillName}</strong></span>
                </div>
                <div class="filter-stat">
                    <i class="fas fa-project-diagram"></i>
                    <span>Projetos encontrados: <strong>${data.projects.length}</strong></span>
                </div>
                <div class="filter-stat">
                    <i class="fas fa-clock"></i>
                    <span>Experiência total: <strong>${experienceText}</strong></span>
                </div>
            </div>
        `;

        document.getElementById('total-projects').textContent = data.projects.length;

        if (data.projects.length === 0) {
            container.innerHTML = `
                <div style="grid-column: 1/-1; text-align: center; padding: 40px; color: #7f8c8d;">
                    <i class="fas fa-inbox" style="font-size: 3rem; margin-bottom: 15px; color: #bdc3c7;"></i>
                    <p style="font-size: 1.2rem;">Nenhum projeto encontrado com essa tecnologia.</p>
                </div>
            `;
            return;
        }

        renderProjects(data.projects, skillId);
    } catch (error) {
        console.error('Erro ao carregar projetos por skill:', error);
        handleProjectsError();
    }
}

//  Carregar TODOS os Projetos
async function loadAllProjects() {
    try {
        console.log('Carregando todos os projetos...');
        const response = await fetch(`${API_BASE_URL}/project/projects-with-company`);
        if (!response.ok) throw new Error(`Erro na API: ${response.status}`);

        const projects = await response.json();
        console.log('Projetos carregados:', projects.length);

        document.getElementById('total-projects').textContent = projects.length;
        const container = document.getElementById('projects-container');
        container.innerHTML = '';

        if (projects.length === 0) {
            container.innerHTML = `
                <div style="grid-column: 1/-1; text-align: center; padding: 40px; color: #7f8c8d;">
                    <i class="fas fa-inbox" style="font-size: 3rem; margin-bottom: 15px; color: #bdc3c7;"></i>
                    <p style="font-size: 1.2rem;">Nenhum projeto cadastrado.</p>
                </div>
            `;
            return;
        }

        renderProjects(projects);
    } catch (error) {
        console.error('Erro ao carregar projetos:', error);
        handleProjectsError();
    }
}

// [NOVA FUNÇÃO] Renderizar projetos (reutilizável)
function renderProjects(projects, skillId = null) {
    const container = document.getElementById('projects-container');
    const sortedProjects = [...projects].sort((a, b) => new Date(b.startDate) - new Date(a.startDate));

    sortedProjects.forEach(project => {
        const startDate = new Date(project.startDate);
        const endDate = project.endDate ? new Date(project.endDate) : new Date();
        const duration = calculateDuration(startDate, endDate);
        const companyName = project.companyName || project.experience?.company || 'Empresa não informada';
        const isCurrentProject = !project.endDate;

        const card = document.createElement('div');
        card.className = 'project-card';
        card.innerHTML = `
            <div class="project-header">
                ${isCurrentProject ? '<span class="project-current-badge"><i class="fas fa-circle"></i> Projeto Atual</span>' : ''}
                <div class="project-name">${project.name}</div>
                <div class="project-position">${project.position}</div>
                <div class="project-meta">
                    <span class="project-company-tag">
                        <i class="fas fa-building"></i> ${companyName}
                    </span>
                    <span class="project-duration">
                        <i class="fas fa-clock"></i> ${duration}
                    </span>
                </div>
            </div>
            <p class="project-description">${project.description}</p>
            <div class="skills-tags" id="project-skills-${project.id}"></div>
        `;
        container.appendChild(card);

        // Adicionar skills ao card
        const skillsContainer = document.getElementById(`project-skills-${project.id}`);
        if (project.skills && project.skills.length > 0) {
            project.skills.forEach(skill => {
                const tag = document.createElement('span');
                tag.className = `skill-tag ${getCategoryClass(skill.category)}`;
                tag.textContent = skill.name;

                // Destacar a skill filtrada
                if (skillId && skill.id === skillId) {
                    tag.style.boxShadow = '0 0 0 3px rgba(102, 126, 234, 0.3)';
                    tag.style.transform = 'scale(1.05)';
                }

                skillsContainer.appendChild(tag);
            });
        }
    });
}

//  Tratamento de erro para projetos
function handleProjectsError() {
    const container = document.getElementById('projects-container');
    const filterInfo = document.getElementById('filter-info');

    if (filterInfo) filterInfo.style.display = 'none';

    container.innerHTML = `
        <div style="grid-column: 1/-1; text-align: center; padding: 40px; color: #e74c3c;">
            <i class="fas fa-exclamation-triangle" style="font-size: 3rem; margin-bottom: 15px;"></i>
            <p style="font-size: 1.2rem;">Erro ao carregar projetos. Tente novamente.</p>
        </div>
    `;
}

//  Carregar Skills
async function loadSkills() {
    try {
        const response = await fetch(`${API_BASE_URL}/skill`);
        if (!response.ok) throw new Error(`Erro de rede: ${response.status}`);

        const skills = await response.json();
        document.getElementById('total-skills').textContent = skills.length;

        const list = document.getElementById('skills-list');
        if (!list) return;

        list.innerHTML = '';
        skills.sort((a, b) => new Date(a.startDate) - new Date(b.startDate));

        skills.forEach(skill => {
            // Usa o IdProficiencyLevel como quantidade de estrelas
            const starsHtml = renderStars(skill.idProficiencyLevel);

            const item = document.createElement('div');
            item.className = 'skill-item';
            item.innerHTML = `
                <div class="skill-name-row">
                    <span class="skill-name">${skill.name}</span>
                    <div class="skill-rating" title="Proficiência: ${getProficiencyLabel(skill.idProficiencyLevel)}">
                        ${starsHtml}
                    </div>
                </div>
            `;
            list.appendChild(item);
        });
    } catch (error) {
        console.error('Erro ao carregar skills:', error);
    }
}

// Função auxiliar para obter o label da proficiência
function getProficiencyLabel(level) {
    const labels = {
        1: 'Beginner',
        2: 'Intermediate',
        3: 'Competent',
        4: 'Proficient',
        5: 'Expert'
    };
    return labels[level] || 'Not Rated';
}

// Renderiza as estrelas baseado no IdProficiencyLevel (1-5)
function renderStars(proficiencyLevel) {
    let starsHtml = '';
    const maxStars = 5;

    for (let i = 1; i <= maxStars; i++) {
        if (i <= proficiencyLevel) {
            starsHtml += '<i class="fas fa-star filled-star"></i>';
        } else {
            starsHtml += '<i class="far fa-star empty-star"></i>';
        }
    }
    return starsHtml;
}

//  Carregar Certificações
async function loadCertifications() {
    try {
        const response = await fetch(`${API_BASE_URL}/certification`);
        if (!response.ok) throw new Error(`Erro de rede: ${response.status}`);

        const certs = await response.json();
        document.getElementById('total-certs').textContent = certs.length;
    } catch (error) {
        console.error('Erro ao carregar certificações:', error);
    }
}

//  Funções Auxiliares
function calculateDuration(start, end) {
    // Garante que as datas são do mesmo tipo
    const startDate = new Date(start);
    const endDate = new Date(end);

    // Calcula diferença total em meses considerando dias
    let months = (endDate.getFullYear() - startDate.getFullYear()) * 12;
    months += endDate.getMonth() - startDate.getMonth();

    // Ajusta para considerar os dias
    if (endDate.getDate() < startDate.getDate()) {
        months--;
    }

    // Garante pelo menos 1 mês para períodos curtos
    if (months === 0 && (endDate - startDate) > 0) {
        months = 1;
    }

    const years = Math.floor(months / 12);
    const remainingMonths = months % 12;

    if (years > 0 && remainingMonths > 0) {
        return `${years} ano${years > 1 ? 's' : ''} e ${remainingMonths} ${remainingMonths > 1 ? 'meses' : 'mês'}`;
    } else if (years > 0) {
        return `${years} ano${years > 1 ? 's' : ''}`;
    } else {
        return `${months} ${months > 1 ? 'meses' : 'mês'}`;
    }
}

function formatDate(date) {
    return date.toLocaleDateString('pt-BR', { month: 'short', year: 'numeric' });
}

function getCategoryClass(category) {
    const categoryName = typeof category === 'string' ? category.toLowerCase() : '';
    const map = {
        'backend': 'backend',
        'frontend': 'frontend',
        'database': 'database',
        'cloud': 'cloud',
        'management': 'management',
        'programminglanguage': 'programminglanguage',
        'devops': 'devops'
    };
    return map[categoryName] || 'backend';
}

//  Download PDF
/**
 * Chama a rota do backend e inicia o download do arquivo PDF.
 */
async function downloadPDF() {


    const downloadButton = document.querySelector('.btn-download');

    if (downloadButton) {
        downloadButton.disabled = true;
        downloadButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Baixando...';
    }

    try {
        // 1. Chama a rota de download usando GET, sem duplicação de /api/
        const response = await fetch(`${API_BASE_URL}/Curriculum/download`, {
            method: 'GET' // Mantenha GET, conforme seu Controller C#
        });

        if (!response.ok) {
            console.error('Erro de resposta do servidor:', response.status);
            const errorText = await response.text();
            throw new Error(`Falha ao obter o PDF do servidor. Status: ${response.status}. Detalhe: ${errorText.substring(0, 100)}...`);
        }
        const contentDisposition = response.headers.get('Content-Disposition');
        let fileName = 'curriculo_download.pdf'; // Fallback

        if (contentDisposition) {
            // Expressão regular para buscar o nome do arquivo.
            // Busca por filename* (UTF-8) primeiro e, se falhar, por filename=.
            const filenameMatch = contentDisposition.match(/filename\*=UTF-8''(.+?)(;|$)|filename="?(.+?)"?(;|$)/i);

            if (filenameMatch && (filenameMatch[1] || filenameMatch[3])) {
                // Decodifica o nome do arquivo se estiver no formato filename* (UTF-8)
                const encodedFilename = filenameMatch[1] || filenameMatch[3];
                // Usa decodeURIComponent para tratar espaços e caracteres especiais (se for filename*)
                fileName = decodeURIComponent(encodedFilename.replace(/"/g, '').split(';')[0]);
            }
        }

        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.href = url;

        // CORREÇÃO ESSENCIAL: Garante que o navegador BAiXE o conteúdo em vez de navegar para ele.
        // O nome do arquivo será sobrescrito pelo backend.
        
        a.download = fileName;
        document.body.appendChild(a);
        a.click(); // Dispara o download

        // Limpa
        a.remove();
        window.URL.revokeObjectURL(url);


    } catch (error) {
        console.error('Erro ao baixar o PDF:', error);
        alert('Ocorreu um erro ao tentar baixar o arquivo. Tente novamente.');
    } finally {
        if (downloadButton) {
            downloadButton.disabled = false;
            downloadButton.innerHTML = '<i class="fas fa-download"></i> Baixar Currículo em PDF';
        }
    }
}
//  Form de sugestões
document.addEventListener('DOMContentLoaded', function () {
    const suggestionsForm = document.getElementById('suggestions-form');
    if (suggestionsForm) {
        suggestionsForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            const loading = document.getElementById('suggestion-loading');
            const form = e.target;

            if (loading) loading.classList.add('show');
            setTimeout(() => {
                alert('Obrigado pela sua sugestão! Ela será analisada em breve.');
                form.reset();
                if (loading) loading.classList.remove('show');
            }, 1500);
        });
    }

    // Inicializar seção de currículo dedicado
    initializeDedicatedCurriculum();
});

// Funções para currículo dedicado
function initializeDedicatedCurriculum() {
    const section = document.getElementById('dedicated-curriculum-section');
    const loginMessage = document.getElementById('dedicated-curriculum-login-message');
    const form = document.getElementById('dedicated-curriculum-form');

    if (!section) return;

    // Verificar autenticação
    const token = localStorage.getItem('token');
    const isAuthenticated = !!token;

    section.style.display = 'block';

    if (isAuthenticated) {
        if (loginMessage) loginMessage.style.display = 'none';
        if (form) {
            form.style.display = 'block';
            // Remover listener anterior se existir e adicionar novo
            const newForm = form.cloneNode(true);
            form.parentNode.replaceChild(newForm, form);
            document.getElementById('dedicated-curriculum-form').addEventListener('submit', handleDedicatedCurriculumSubmit);
        }
    } else {
        if (loginMessage) loginMessage.style.display = 'block';
        if (form) form.style.display = 'none';
    }
}

function getAuthToken() {
    return localStorage.getItem('token');
}

function isAuthenticated() {
    return !!getAuthToken();
}

async function handleDedicatedCurriculumSubmit(e) {
    e.preventDefault();

    if (!isAuthenticated()) {
        showDedicatedCurriculumError('Você precisa estar autenticado para usar esta funcionalidade.');
        return;
    }

    const form = e.target;
    const companyName = document.getElementById('company-name')?.value || '';
    const jobDescription = document.getElementById('job-description')?.value || '';

    if (!jobDescription || jobDescription.trim().length < 50) {
        showDedicatedCurriculumError('A descrição da vaga deve ter pelo menos 50 caracteres.');
        return;
    }

    const loading = document.getElementById('dedicated-curriculum-loading');
    const errorDiv = document.getElementById('dedicated-curriculum-error');
    const submitBtn = document.getElementById('generate-curriculum-btn');

    // Mostrar loading e esconder erro
    if (loading) loading.style.display = 'flex';
    if (errorDiv) errorDiv.style.display = 'none';
    if (submitBtn) {
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Gerando...';
    }

    try {
        const token = getAuthToken();
        const response = await fetch(`${API_BASE_URL}/dedicated-curriculum/generate`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
                jobDescription: jobDescription.trim(),
                companyName: companyName.trim() || null
            })
        });

        if (response.status === 401) {
            showDedicatedCurriculumError('Sua sessão expirou. Por favor, faça login novamente.');
            localStorage.removeItem('token');
            return;
        }

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ message: 'Erro ao gerar currículo' }));
            showDedicatedCurriculumError(errorData.message || 'Erro ao gerar currículo. Tente novamente.');
            return;
        }

        // Download do PDF
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;

        // Extrair nome do arquivo do header Content-Disposition
        const contentDisposition = response.headers.get('Content-Disposition');
        let fileName = 'Curriculo_Dedicado.pdf';
        if (contentDisposition) {
            const fileNameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
            if (fileNameMatch && fileNameMatch[1]) {
                fileName = fileNameMatch[1].replace(/['"]/g, '');
                fileName = decodeURIComponent(fileName);
            }
        }

        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(url);

        // Limpar formulário
        form.reset();

    } catch (error) {
        console.error('Erro ao gerar currículo dedicado:', error);
        showDedicatedCurriculumError('Erro ao conectar com o servidor. Verifique sua conexão e tente novamente.');
    } finally {
        if (loading) loading.style.display = 'none';
        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.innerHTML = '<i class="fas fa-magic"></i> Gerar Currículo Personalizado';
        }
    }
}

function showDedicatedCurriculumError(message) {
    const errorDiv = document.getElementById('dedicated-curriculum-error');
    if (errorDiv) {
        errorDiv.textContent = message;
        errorDiv.style.display = 'block';
    }
}

// ========== FUNÇÕES DE AUTENTICAÇÃO ==========

function checkAuthStatus() {
    const token = localStorage.getItem('token');
    const username = localStorage.getItem('username');
    const loginBtn = document.getElementById('login-btn');
    const userInfo = document.getElementById('user-info');
    const userName = document.getElementById('user-name');

    if (token && username) {
        // Usuário está logado
        if (loginBtn) loginBtn.style.display = 'none';
        if (userInfo) userInfo.style.display = 'flex';
        if (userName) userName.textContent = username;
        
        // Atualizar seção de currículo dedicado
        initializeDedicatedCurriculum();
    } else {
        // Usuário não está logado
        if (loginBtn) loginBtn.style.display = 'block';
        if (userInfo) userInfo.style.display = 'none';
    }
}

function openLoginModal() {
    const modal = document.getElementById('login-modal');
    if (modal) {
        modal.style.display = 'flex';
        document.body.style.overflow = 'hidden'; // Prevenir scroll
        
        // Limpar formulário e erros ao abrir
        const form = document.getElementById('login-form');
        if (form) form.reset();
        const errorDiv = document.getElementById('login-error');
        if (errorDiv) {
            errorDiv.style.display = 'none';
            errorDiv.textContent = '';
        }
        const loadingDiv = document.getElementById('login-loading');
        if (loadingDiv) loadingDiv.style.display = 'none';
        
        // Focar no campo de email
        const emailInput = document.getElementById('login-email');
        if (emailInput) {
            setTimeout(() => emailInput.focus(), 100);
        }
        
        // Fechar modal ao clicar fora
        modal.onclick = function(event) {
            if (event.target === modal) {
                closeLoginModal();
            }
        };
        
        // Fechar modal com ESC
        document.addEventListener('keydown', handleModalEscape);
    }
}

function handleModalEscape(event) {
    if (event.key === 'Escape') {
        const modal = document.getElementById('login-modal');
        if (modal && modal.style.display === 'flex') {
            closeLoginModal();
        }
    }
}

function closeLoginModal() {
    const modal = document.getElementById('login-modal');
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = 'auto';
        
        // Limpar formulário e erros ao fechar
        const form = document.getElementById('login-form');
        if (form) form.reset();
        const errorDiv = document.getElementById('login-error');
        if (errorDiv) {
            errorDiv.style.display = 'none';
            errorDiv.textContent = '';
        }
        const loadingDiv = document.getElementById('login-loading');
        if (loadingDiv) loadingDiv.style.display = 'none';
        
        // Remover listener do ESC
        document.removeEventListener('keydown', handleModalEscape);
    }
}

// ========== FUNÇÕES DE AUTENTICAÇÃO ==========

async function handleLogin(event) {
    event.preventDefault();
    
    const email = document.getElementById('login-email').value.trim();
    const password = document.getElementById('login-password').value;
    const errorDiv = document.getElementById('login-error');
    const loadingDiv = document.getElementById('login-loading');
    const submitBtn = document.getElementById('login-submit-btn');
    const form = document.getElementById('login-form');

    // Esconder erro anterior
    if (errorDiv) {
        errorDiv.style.display = 'none';
        errorDiv.textContent = '';
    }

    // Validar campos
    if (!email || !password) {
        showLoginError('Por favor, preencha todos os campos.');
        return;
    }

    // Mostrar loading
    if (loadingDiv) loadingDiv.style.display = 'flex';
    if (submitBtn) {
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Entrando...';
    }

    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                email: email,
                password: password
            })
        });

        const data = await response.json().catch(() => ({}));

        if (!response.ok) {
            const errorMessage = data.message || 'Erro ao fazer login. Verifique suas credenciais.';
            showLoginError(errorMessage);
            return;
        }

        // Login bem-sucedido
        if (data.token) {
            // Salvar tokens
            localStorage.setItem('token', data.token);
            localStorage.setItem('refreshToken', data.refreshToken || '');
            localStorage.setItem('username', data.username || email.split('@')[0]);
            localStorage.setItem('userRole', data.role || 'User');
            if (data.email) {
                localStorage.setItem('email', data.email);
            }

            // Atualizar UI
            checkAuthStatus();
            
            // Fechar modal
            closeLoginModal();
            
            // Limpar formulário
            form.reset();
            
            // Mostrar mensagem de sucesso
            alert(`Bem-vindo(a), ${data.username || email.split('@')[0]}!`);
        } else {
            showLoginError('Resposta inválida do servidor.');
        }
    } catch (error) {
        console.error('Erro ao fazer login:', error);
        showLoginError('Erro ao conectar com o servidor. Verifique sua conexão e tente novamente.');
    } finally {
        // Esconder loading
        if (loadingDiv) loadingDiv.style.display = 'none';
        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.innerHTML = '<i class="fas fa-sign-in-alt"></i> Entrar';
        }
    }
}

function showLoginError(message) {
    const errorDiv = document.getElementById('login-error');
    if (errorDiv) {
        errorDiv.textContent = message;
        errorDiv.style.display = 'block';
        // Scroll para o erro
        errorDiv.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }
}

// ========== FUNÇÕES DE LOGIN COM GOOGLE ==========

function loginWithGoogle() {
    // Fechar modal antes de redirecionar
    closeLoginModal();
    // Redirecionar para o endpoint de login do Google
    window.location.href = `${API_BASE_URL}/auth/google-login`;
}

function handleGoogleCallback() {
    const urlParams = new URLSearchParams(window.location.search);
    const authStatus = urlParams.get('auth');
    const tokensParam = urlParams.get('tokens');
    const error = urlParams.get('error');
    const details = urlParams.get('details');

    if (error) {
        let errorMessage = 'Erro ao fazer login com Google.';
        switch (error) {
            case 'google_auth_failed':
                errorMessage = 'Falha na autenticação com Google.';
                if (details) {
                    errorMessage += `\n\nDetalhes: ${decodeURIComponent(details)}`;
                } else {
                    errorMessage += ' Tente novamente.';
                }
                break;
            case 'google_info_incomplete':
                errorMessage = 'Informações do Google incompletas. Tente novamente.';
                break;
            case 'google_callback_error':
                errorMessage = 'Erro no processamento do login.';
                if (details) {
                    errorMessage += `\n\nDetalhes: ${decodeURIComponent(details)}`;
                } else {
                    errorMessage += ' Tente novamente.';
                }
                break;
        }
        
        // Usar console para debug também
        console.error('Erro no login Google:', error, details ? decodeURIComponent(details) : '');
        
        alert(errorMessage);
        // Limpar URL
        window.history.replaceState({}, document.title, window.location.pathname);
        return;
    }

    if (authStatus === 'success' && tokensParam) {
        try {
            const decodedTokens = decodeURIComponent(tokensParam);
            const tokensJson = atob(decodedTokens);
            const tokenData = JSON.parse(tokensJson);

            // Salvar tokens
            localStorage.setItem('token', tokenData.token);
            localStorage.setItem('refreshToken', tokenData.refreshToken);
            localStorage.setItem('username', tokenData.username);
            localStorage.setItem('userRole', tokenData.role);
            if (tokenData.email) {
                localStorage.setItem('email', tokenData.email);
            }

            // Atualizar UI
            checkAuthStatus();
            alert(`Bem-vindo(a), ${tokenData.username}!`);

            // Limpar URL
            window.history.replaceState({}, document.title, window.location.pathname);
        } catch (e) {
            console.error('Erro ao processar tokens do Google:', e);
            alert('Erro ao processar informações de login. Tente novamente.');
            window.history.replaceState({}, document.title, window.location.pathname);
        }
    }
}

async function handleLogout() {
    if (!confirm('Deseja realmente sair?')) {
        return;
    }

    try {
        const refreshToken = localStorage.getItem('refreshToken');
        
        if (refreshToken) {
            // Tentar fazer logout no servidor
            const token = localStorage.getItem('token');
            if (token) {
                await fetch(`${API_BASE_URL}/auth/logout`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`
                    },
                    body: JSON.stringify({
                        refreshToken: refreshToken
                    })
                }).catch(err => {
                    console.error('Erro ao fazer logout no servidor:', err);
                    // Continuar mesmo se falhar
                });
            }
        }
    } catch (error) {
        console.error('Erro ao fazer logout:', error);
    } finally {
        // Limpar localStorage
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('username');
        localStorage.removeItem('email');
        localStorage.removeItem('userRole');
        
        // Atualizar UI
        checkAuthStatus();
        alert('Logout realizado com sucesso!');
    }
}