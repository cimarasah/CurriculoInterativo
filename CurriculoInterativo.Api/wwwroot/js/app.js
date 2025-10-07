const API_BASE_URL = 'http://localhost:5083/api';

// Inicialização
document.addEventListener('DOMContentLoaded', function () {
    loadDashboard();
    setupFilterListeners();
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

//  Timeline de Experiências
async function loadExperiencesTimeline() {
    try {
        const response = await fetch(`${API_BASE_URL}/experience`);
        if (!response.ok) throw new Error(`Erro de rede: ${response.status}`);

        const experiences = await response.json();
        const timeline = document.getElementById('timeline');
        if (!timeline) return;

        timeline.innerHTML = '';
        const sortedExperiences = [...experiences].sort((a, b) => new Date(a.startDate) - new Date(b.startDate));

        sortedExperiences.forEach((exp, index) => {
            const startDate = new Date(exp.startDate);
            const endDate = exp.endDate ? new Date(exp.endDate) : new Date();
            const duration = calculateDuration(startDate, endDate);
            const isCurrentJob = !exp.endDate;

            const item = document.createElement('div');
            item.className = 'timeline-journey-item';
            item.innerHTML = `
                <div class="timeline-content">
                    ${isCurrentJob ? '<span class="timeline-current-badge"><i class="fas fa-briefcase"></i> Atual</span>' : ''}
                    <div class="timeline-date">
                        <i class="fas fa-calendar-alt"></i>
                        ${formatDate(startDate)} - ${isCurrentJob ? 'Presente' : formatDate(endDate)}
                    </div>
                    <h3 class="timeline-project">${exp.company}</h3>
                    <div class="timeline-location">
                        <i class="fas fa-map-marker-alt"></i>
                        ${exp.location || 'Localização não informada'}
                    </div>
                    <div class="timeline-duration">
                        <i class="fas fa-clock"></i>
                        ${duration}
                    </div>
                    ${exp.description ? `<p class="timeline-description">${exp.description}</p>` : ''}
                </div>
                <div class="timeline-dot"></div>
            `;
            timeline.appendChild(item);
        });
    } catch (error) {
        console.error('Erro ao carregar timeline de experiências:', error);
    }
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
            ? `${data.years} ano${data.years > 1 ? 's' : ''}${data.months > 0 ? ` e ${data.months} mês${data.months > 1 ? 'es' : ''}` : ''}`
            : `${data.months} mês${data.months > 1 ? 'es' : ''}`;

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
            const startDate = new Date(skill.startDate);
            const starRating = calculateStarRating(startDate);
            const starsHtml = renderStars(starRating);

            const item = document.createElement('div');
            item.className = 'skill-item';
            item.innerHTML = `
                <div class="skill-name-row">
                    <span class="skill-name">${skill.name}</span>
                    <div class="skill-rating" title="Proficiência: ${starRating.toFixed(1)} estrelas">
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
    const months = (end.getFullYear() - start.getFullYear()) * 12 + (end.getMonth() - start.getMonth());
    const years = Math.floor(months / 12);
    const remainingMonths = months % 12;

    if (years > 0 && remainingMonths > 0) {
        return `${years} ano${years > 1 ? 's' : ''} e ${remainingMonths} mês${remainingMonths > 1 ? 'es' : ''}`;
    } else if (years > 0) {
        return `${years} ano${years > 1 ? 's' : ''}`;
    } else {
        return `${remainingMonths} mês${remainingMonths > 1 ? 'es' : ''}`;
    }
}

function calculateStarRating(startDate) {
    const now = new Date();
    const diffTime = Math.abs(now - startDate);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    const yearsExperience = diffDays / 365.25;
    return Math.min(5, yearsExperience);
}

function renderStars(rating) {
    let starsHtml = '';
    const maxStars = 5;
    for (let i = 1; i <= maxStars; i++) {
        if (rating >= i) {
            starsHtml += '<i class="fas fa-star filled-star"></i>';
        } else if (rating >= i - 0.5) {
            starsHtml += '<i class="fas fa-star-half-alt filled-star"></i>';
        } else {
            starsHtml += '<i class="far fa-star empty-star"></i>';
        }
    }
    return starsHtml;
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
function downloadPDF() {
    alert('Funcionalidade de download de PDF será implementada em breve!');
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
});