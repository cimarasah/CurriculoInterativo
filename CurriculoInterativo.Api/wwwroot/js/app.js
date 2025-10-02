const API_BASE_URL = 'http://localhost:5083/api';

// Inicialização
document.addEventListener('DOMContentLoaded', function () {
    loadDashboard();
});

async function loadDashboard() {
    try {
        await Promise.all([
            loadExperiences(),
            loadProjects(),
            loadSkills(),
            loadCertifications()
        ]);
    } catch (error) {
        console.error('Erro ao carregar dashboard:', error);
    }
}

// Timeline de Experiências
async function loadExperiences() {
    try {
        const response = await fetch(`${API_BASE_URL}/experience`);

        if (!response.ok) {
            throw new Error(`Erro de rede: ${response.status} ${response.statusText}`);
        }

        const experiences = await response.json();
        const logoCollection = document.getElementById('company-logo-collection');

        if (!logoCollection) {
            console.error("Elemento 'company-logo-collection' não encontrado. Verifique seu index.html.");
            return;
        }

        logoCollection.innerHTML = '';
        experiences.sort((a, b) => new Date(b.startDate) - new Date(a.startDate));

        experiences.forEach(exp => {
            const logoItem = document.createElement('div');
            logoItem.className = 'company-logo-item';
            logoItem.setAttribute('title', exp.company);

            // Determina se usará a imagem ou o nome da empresa
            if (exp.imgLogo) {
                // Caso 1: Logo em Base64 está disponível
                const imgBase64Src = `data:image/jpeg;base64,${exp.imgLogo}`;
                logoItem.innerHTML = `
                    <img src="${imgBase64Src}" alt="${exp.company} Logo" class="logo-image">
                    <span class="logo-name">${exp.company}</span>
                `;
            } else {
                // Caso 2: Logo NÃO está disponível -> Exibir Sigla da Empresa

                // Função para criar a sigla (ex: "ACME Corp" -> "AC")
                const initials = exp.company
                    .split(' ') // Divide por espaço
                    .map(word => word[0]) // Pega a primeira letra de cada palavra
                    .join('') // Junta as letras
                    .toUpperCase()
                    .slice(0, 2); // Limita a duas letras (para não poluir)

                logoItem.innerHTML = `
                    <div class="logo-initials-fallback">
                        ${initials}
                    </div>
                    <span class="logo-name">${exp.company}</span>
                `;
            }

            logoCollection.appendChild(logoItem);
        });

    } catch (error) {
        console.error('Erro ao carregar logos das empresas:', error);
    }
}
// Projetos E Timeline de Projetos
async function loadProjects() {
    try {
        const response = await fetch(`${API_BASE_URL}/project/projects-with-company`);
        const projects = await response.json();

        document.getElementById('total-projects').textContent = projects.length;

        const grid = document.getElementById('projects-grid');
        grid.innerHTML = '';

        const timeline = document.getElementById('timeline');
        timeline.innerHTML = ''; // Limpa o timeline antes de preencher com projetos

        // Ordena os projetos pela data de início (mais recente primeiro para a timeline)
        projects.sort((a, b) => new Date(b.startDate) - new Date(a.startDate));

        projects.forEach(project => {
            const startDate = new Date(project.startDate);
            const endDate = project.endDate ? new Date(project.endDate) : new Date();
            const duration = calculateDuration(startDate, endDate);

            // --- Construção do Card de Projeto (Para a seção de Projetos em Destaque) ---
            const card = document.createElement('div');
            card.className = 'project-card';
            card.innerHTML = `
                        <div class="project-header">
                            <div class="project-name">${project.name}</div>
                            <div class="project-position">${project.position}</div>
                            <div class="project-duration">
                                <i class="fas fa-clock"></i> ${duration}
                            </div>
                        </div>
                        <p style="color: #555; margin-bottom: 15px;">${project.description}</p>
                        <div class="skills-tags" id="project-skills-${project.id}"></div>
                    `;
            grid.appendChild(card);

            // Adicionar skills ao card
            const skillsContainer = document.getElementById(`project-skills-${project.id}`);
            if (project.skills && project.skills.length > 0) {
                project.skills.forEach(skill => {
                    const tag = document.createElement('span');
                    tag.className = `skill-tag ${getCategoryClass(skill.category)}`;
                    tag.textContent = skill.name;
                    skillsContainer.appendChild(tag);
                });
            }

            
            const companyName = project.experience.company || 'Empresa não informada'; 

            const timelineItem = document.createElement('div');
            timelineItem.className = 'timeline-item';
            timelineItem.innerHTML = `
                        <div class="timeline-dot"></div>
                        <div class="timeline-content">
                            <div class="timeline-date">
                                ${formatDate(startDate)} - ${project.endDate ? formatDate(endDate) : 'Presente'} (${duration})
                            </div>
                            <div class="timeline-project">${project.name}</div> 
                            <div class="timeline-location">
                                <i class="fas fa-building"></i> ${companyName}
                            </div>
                            <p style="margin-top: 10px; color: #555;">Posição: ${project.position}</p> 
                        </div>
                    `;
            timeline.appendChild(timelineItem);
        });
    } catch (error) {
        console.error('Erro ao carregar projetos e timeline:', error);
    }
}

// Skills
async function loadSkills() {
    try {
        const response = await fetch(`${API_BASE_URL}/skill`);
        const skills = await response.json();

        document.getElementById('total-skills').textContent = skills.length;

        const list = document.getElementById('skills-list');
        list.innerHTML = '';

        skills.sort((a, b) => new Date(a.startDate) - new Date(b.startDate));

        skills.forEach(skill => {
            const startDate = new Date(skill.startDate);
            const now = new Date();
            const timeExperience = calculateDetailedDuration(startDate, now);
            const proficiency = calculateProficiency(startDate);

            const item = document.createElement('div');
            item.className = 'skill-item';
            item.innerHTML = `
                        <div class="skill-header">
                            <span class="skill-name">${skill.name}</span>
                            <span class="skill-time">${timeExperience}</span>
                        </div>
                        <div class="skill-bar">
                            <div class="skill-progress" style="width: 0%" data-width="${proficiency}%"></div>
                        </div>
                    `;
            list.appendChild(item);
        });

        // Animar barras
        setTimeout(() => {
            document.querySelectorAll('.skill-progress').forEach(bar => {
                bar.style.width = bar.dataset.width;
            });
        }, 100);
    } catch (error) {
        console.error('Erro ao carregar skills:', error);
    }
}

// Certificações
async function loadCertifications() {
    try {
        const response = await fetch(`${API_BASE_URL}/certification`);
        const certs = await response.json();

        document.getElementById('total-certs').textContent = certs.length;
    } catch (error) {
        console.error('Erro ao carregar certificações:', error);
    }
}

// Funções auxiliares
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

function calculateDetailedDuration(start, end) {
    const years = end.getFullYear() - start.getFullYear();
    const months = end.getMonth() - start.getMonth();
    const totalMonths = years * 12 + months;

    const y = Math.floor(totalMonths / 12);
    const m = totalMonths % 12;

    if (y > 0 && m > 0) {
        return `${y} ano${y > 1 ? 's' : ''} e ${m} mês${m > 1 ? 'es' : ''}`;
    } else if (y > 0) {
        return `${y} ano${y > 1 ? 's' : ''}`;
    } else {
        return `${m} mês${m > 1 ? 'es' : ''}`;
    }
}

function calculateProficiency(startDate) {
    const now = new Date();
    const months = (now.getFullYear() - startDate.getFullYear()) * 12 + (now.getMonth() - startDate.getMonth());
    return Math.min(100, (months / 60) * 100);
}

function formatDate(date) {
    return date.toLocaleDateString('pt-BR', { month: 'short', year: 'numeric' });
}

function getCategoryClass(category) {
    const map = {
        0: 'backend',
        1: 'frontend',
        2: 'database',
        3: 'cloud',
        4: 'backend'
    };
    return map[category] || 'backend';
}

// Download PDF
function downloadPDF() {
    alert('Funcionalidade de download de PDF será implementada em breve!');
}

// Form de sugestões
document.getElementById('suggestions-form').addEventListener('submit', async function (e) {
    e.preventDefault();

    const loading = document.getElementById('suggestion-loading');
    const form = e.target;

    loading.classList.add('show');

    // Simular envio
    setTimeout(() => {
        alert('Obrigado pela sua sugestão! Ela será analisada em breve.');
        form.reset();
        loading.classList.remove('show');
    }, 1500);
});